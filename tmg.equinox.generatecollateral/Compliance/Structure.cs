using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime;
using System.Net;

using WebSupergoo.ABCpdf11;
using WebSupergoo.ABCpdf11.Objects;
using WebSupergoo.ABCpdf11.Atoms;
using WebSupergoo.ABCpdf11.Operations;
using WebSupergoo.ABCpdf11.Elements;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.generatecollateral
{
    public sealed class Structure
    {
        private Doc _doc = null;
        private Catalog _catalog = null;
        private ParentTree _parentTree = null;
        private Dictionary<int, int> _pages = null;
        private string[] _contents = null;
        private List<StreamObject> _layers = null;
        private StructureElementElement _root = null;
        private bool _fixAccessibility = false;
        private static readonly ILog _logger = LogProvider.For<ComplianceService>();
        public Structure()
        {
        }

        public Structure(bool fixAccessibility)
        {
            _fixAccessibility = fixAccessibility;
        }
        public Catalog Catalog { get { return _catalog; } }
        public StructureElementElement Root { get { return _root; } }
        public ParentTree ParentTree { get { return _parentTree; } }
        public ComplianceDetailInfo ComplianceDetailInfoData { get; set; }
        public void Clear()
        {
            _pages = null;
            _contents = null;
        }

        public void Load(string path)
        {
            _doc = new Doc();


            if (_fixAccessibility)
            {
                var xr = new XReadOptions();
                xr.ReadModule = ReadModuleType.MSOffice;
                xr.Repair = true;
                xr.Timeout = 10000000;
                _doc.Read(path, xr);

            }
            else
            {
                _doc.Read(path);
            }
            Load(_doc);
        }

        public void FixAccessibility(string dst, string title)
        {
            AccessibilityOperation op = new AccessibilityOperation(_doc);

            op.FindFooters = true;
            op.FindHeaders = true;
            op.FindLists = true;
            op.FindStructure = true;
            op.FindTables = true;


            op.PageContents.IncludeAnnotations = true;
            op.PageContents.RegenerateUnicode = true;
            op.PageContents.IncludeColor = true;

            op.Layout = AccessibilityOperation.LayoutMethod.Recursive;
            op.FixFonts = true;

            op.FixMetadata = true;

            Metadata md = _doc.ObjectSoup.Catalog.Metadata;

            if (md == null)
            {
                md = new Metadata(_doc.ObjectSoup);
                _doc.ObjectSoup.Catalog.Metadata = md;
            }
            md.InfoAuthor = config.Config.GetApplicationName();
            md.InfoTitle = title;

            if (ComplianceDetailInfoData != null && !string.IsNullOrEmpty(ComplianceDetailInfoData.InfoTitle))
            {
                md.InfoTitle = ComplianceDetailInfoData.InfoTitle;
            }
            if (ComplianceDetailInfoData != null && !string.IsNullOrEmpty(ComplianceDetailInfoData.InfoAuthor))
            {
                md.InfoAuthor = ComplianceDetailInfoData.InfoAuthor;
            }

            op.PageContents.AddPages();

            var data = md.GetData();
            string content = System.Text.Encoding.UTF8.GetString(data);
            if (content.Contains("pdfuaid:part") == false)
            {
                var newContent = content.Replace("</rdf:RDF>", @"<rdf:Description rdf:about="""" xmlns:pdfuaid=""http://www.aiim.org/pdfua/ns/id/""><pdfuaid:part>1</pdfuaid:part></rdf:Description></rdf:RDF>");
                md.SetData(System.Text.Encoding.ASCII.GetBytes(newContent));
            }


            FixPartToDocTag();
       
            FixAnnotOnLink();
       
            FixAnnotOnFigure();
       
            
           // FixAnnotOnTableHead();

            FixAnnotOnTable();
            FixAnnotOnTableHead();
            FixAnnotOnTableBodyTHtoTD();
       
            _doc.SetInfo(_doc.Root, "/MarkInfo", "<< /Marked true /UserProperties false /Suspects false >>");
            _doc.SetInfo(_doc.Root, "/ViewerPreferences*/DisplayDocTitle", "true");


            _doc.Save(dst);

        }
        public void FixAnnotOnTableBodyTHtoTD()
        {
            List<StructureElementElement> elements = FindElementsByType("Table");

            foreach (StructureElementElement element in elements)
            {

                if (element.EntryK != null)
                {
                    if (element.EntryK.Count > 1)
                    {
                        var body = (StructureElementElement)element.EntryK[1];
                        if (body.EntryK != null)
                        {
                            var ctr = 0;
                            foreach (StructureElementElement tr in body.EntryK)
                            {

                                if (tr.EntryK != null)
                                {
                                    foreach (StructureElementElement th in tr.EntryK)
                                    {
                                        if (th.EntryS != null)
                                        {
                                            if (th.EntryS != "TD")
                                            {
                                                th.EntryS = "TD";
                                            }
                                        }
                                        else
                                        {
                                            th.EntryS = "TD";
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

            }
        }
        public void FixAnnotOnTable()
        {
            List<StructureElementElement> elements = FindElementsByType("Table");


            foreach (StructureElementElement table in elements)
            {

                //only body
                if (table.EntryK.Count == 1)
                {
                    //create head
                    PageObjectElement page = new PageObjectElement(_doc.ObjectSoup[_doc.Page]);
                    StructureElementElement tHead = new StructureElementElement(page);
                    tHead.EntryType = "StructElem";
                    tHead.EntryS = "THead";
                    tHead.SetParent(table);

                    var bodyCollection = (StructureElementElement)table.EntryK[0];
                    if (bodyCollection.EntryK.Count > 0)
                    {
                        var firstRow = (StructureElementElement)bodyCollection.EntryK[0].Clone();


                        foreach (StructureElementElement th in firstRow.EntryK)
                        {
                            if (th.EntryS != null)
                            {
                                if (th.EntryS != "TH")
                                {
                                    th.EntryS = "TH";
                                }
                            }
                            else
                            {
                                th.EntryS = "TH";
                            }
                        }

                        tHead.EntryK = new ArrayElement<Element>((Element)firstRow);
                        tHead.EntryK.Add((Element)firstRow);

                        bodyCollection.EntryK.RemoveAt(0);
                    }

                    StructureElementElement tbody = null;
                    //swap tbody with THeader
                    foreach (StructureElementElement rows in table.EntryK)
                    {
                        tbody = (StructureElementElement)rows.Clone();
                        break;
                    }
                    if (tbody != null)
                    {
                        table.EntryK.RemoveAt(0);
                        table.EntryK.Add(tbody);
                    }
                }
                if (table.EntryK.Count >= 1)
                {

                    var tHeadCollection = (StructureElementElement)table.EntryK[0];


                    if (tHeadCollection.EntryK.Count == 1 && tHeadCollection.EntryS == "THead")
                    {
                        bool found = false;
                        foreach (StructureElementElement row in tHeadCollection.EntryK)
                        {
                            if (row.EntryK.Count > 0)
                            {
                                try
                                {
                                    if (((WebSupergoo.ABCpdf11.Atoms.NumAtom)new WebSupergoo.ABCpdf11.Elements.ArrayElementDebugView<WebSupergoo.ABCpdf11.Elements.Element>(((WebSupergoo.ABCpdf11.Elements.StructureElementElement)new WebSupergoo.ABCpdf11.Elements.ArrayElementDebugView<WebSupergoo.ABCpdf11.Elements.Element>(row.EntryK).Items[0]).EntryK).Items[0].Atom).Num == 0)
                                    {
                                        row.EntryK.RemoveAt(0);
                                    }
                                }
                                catch (Exception e)
                                {
                                    //leave it blank
                                }

                                try
                                {
                                    ///in special case change in header change to TH to TD
                                    foreach (StructureElementElement cell in row.EntryK)
                                    {
                                        if (cell.EntryS != null)
                                        {
                                            if (cell.EntryS == "TH")
                                            {
                                                cell.EntryS = "TD";
                                            }
                                        }

                                    }
                                }
                                catch (Exception e)
                                {
                                    //leave it blank
                                }
                            }
                            break;
                        }

                    }
                }

                ////removing extra TH under Thead
                //if (table.EntryK.Count >= 1)
                //{

                //    var tHeadCollection = (StructureElementElement)table.EntryK[0];


                //    if (tHeadCollection.EntryK.Count > 0 && tHeadCollection.EntryS == "THead")
                //    {

                //        foreach (StructureElementElement row in tHeadCollection.EntryK)
                //        {
                //            var removeSpan = new List<int>();
                //            int ctr = 0;
                //            if (row.EntryS == "TR")
                //            {
                //                bool found = false;

                //                foreach (StructureElementElement th in row.EntryK)
                //                {

                //                    if (th.EntryS != null)
                //                    {
                //                        if (th.EntryS == "TH")
                //                        {

                //                            if (th.EntryK.Count > 1)
                //                            {
                //                                var spn = (StructureElementElement)th.EntryK[0];
                //                                using (StreamWriter w = File.AppendText(@"d:\log.txt"))
                //                                {
                //                                    w.Write(spn.Atom.ToString() + "\n");
                //                                }
                //                                if (spn.EntryK != null)
                //                                    {
                //                                        if (spn.EntryK.Count > 0)
                //                                        {
                //                                            if (spn.EntryK[0] is Element)
                //                                            {

                //                                                if (spn.EntryK[0].Atom != null)
                //                                                {
                //                                                    if (spn.EntryK[0].Atom is DictAtom)
                //                                                    {
                //                                                        var atoms = (DictAtom)spn.EntryK[0].Atom;
                //                                                        if (atoms.Count == 0)
                //                                                        {
                //                                                            found = true;
                //                                                        }

                //                                                    }
                //                                                }
                //                                            }
                //                                        }

                //                                    }



                //                            }
                //                        }
                //                    }
                //                }
                //                if (found == true)
                //                {
                //                    row.EntryK.RemoveAt(0);
                //                    found = false;
                //                }
                //            }
                //        }
                //    }
                //}
            }

        }
        public void FixAnnotOnTableHead()
        {
            List<StructureElementElement> elements = FindElementsByType("Table");


            foreach (StructureElementElement table in elements)
            {


                foreach (StructureElementElement row in table.EntryK)
                {
                    var removeSpan = new List<int>();
                    int ctr = 0;
                    if (row.EntryK.Count == 1 && ((StructureElementElement)row.EntryK[0]).EntryS == "Span")
                    {
                        ((StructureElementElement)row.EntryK[0]).EntryS = "TH";
                    }
                    else
                    {
                        foreach (StructureElementElement cell in row.EntryK)
                        {
                            if (cell.EntryS != null)
                            {
                                if (cell.EntryS == "Span")
                                    removeSpan.Add(ctr);
                            }
                            ctr++;
                        }
                        foreach (var index in removeSpan)
                        {
                            row.EntryK.RemoveAt(index);
                        }
                    }
                }


                foreach (StructureElementElement row in table.EntryK)
                {
                    var removeSpan = new List<int>();
                    int ctr = 0;
                    if (row.EntryK.Count == 1 && ((StructureElementElement)row.EntryK[0]).EntryS == "Span")
                    {
                        ((StructureElementElement)row.EntryK[0]).EntryS = "TH";
                    }
                    else
                    {
                        foreach (StructureElementElement cell in row.EntryK)
                        {
                            if (cell.EntryS != null)
                            {
                                if (cell.EntryS == "Span")
                                    removeSpan.Add(ctr);
                            }
                            ctr++;
                        }
                        foreach (var index in removeSpan)
                        {
                            row.EntryK.RemoveAt(index);
                        }
                    }
                }
                if (table.EntryK.Count == 2)
                {

                    foreach (StructureElementElement headBody in table.EntryK)
                    {


                        foreach (StructureElementElement tr in headBody.EntryK)
                        {
                            int counter = 0;
                            var removeSpan = new List<int>();
                            foreach (StructureElementElement TDorTH in tr.EntryK)
                            {
                                if (TDorTH.EntryS == "Span")
                                {
                                    removeSpan.Add(counter);
                                    //if (counter==1)
                                    // TDorTH.EntryS = "TH";
                                    //else
                                    //TDorTH.EntryS = "TD";
                                }
                                counter++;

                            }
                            foreach (var index in removeSpan)
                            {
                                tr.EntryK.RemoveAt(index);
                            }
                        }
                    }

                }
                if (table.EntryK.Count == 1)
                {

                    var bodyCollection = (StructureElementElement)table.EntryK[0];


                    if (bodyCollection.EntryK.Count > 0)
                    {
                        var removeSpan = new List<int>();
                        int ctr = 0;
                        if (bodyCollection.EntryS == "TR")
                        {



                            foreach (StructureElementElement th in bodyCollection.EntryK)
                            {
                                if (th.EntryS != null)
                                {
                                    if (th.EntryS != "TH")
                                    {
                                        if (th.EntryS == "Span")
                                            removeSpan.Add(ctr);
                                        else
                                            th.EntryS = "TH";
                                    }
                                }
                                else
                                {
                                    th.EntryS = "TH";
                                }
                                ctr++;
                            }

                            foreach (var index in removeSpan)
                            {
                                bodyCollection.EntryK.RemoveAt(index);
                            }
                        }
                        else
                        {
                            var firstRow = (StructureElementElement)bodyCollection.EntryK[0];


                            foreach (StructureElementElement th in firstRow.EntryK)
                            {
                                if (th.EntryS != null)
                                {
                                    if (th.EntryS != "TH")
                                    {
                                        th.EntryS = "TH";
                                    }
                                }
                                else
                                {
                                    th.EntryS = "TH";
                                }
                            }
                        }
                    }
                }




            }
        }

        private void fixTableHeaderForMultipleRow(StructureElementElement bodyCollection)
        {
            try
            {
    

            if (bodyCollection.EntryK.Count > 0)
            {
                var removeSpan = new List<int>();
                int ctr = 0;

                    if (bodyCollection.EntryS == null)
                        return;

                if (bodyCollection.EntryS == "TR")
                {

                    foreach (StructureElementElement th in bodyCollection.EntryK)
                    {
                        if (th.EntryS != null)
                        {
                            if (th.EntryS != "TH")
                            {
                                if (th.EntryS == "Span")
                                    removeSpan.Add(ctr);
                                else
                                    th.EntryS = "TH";
                            }
                        }
                        else
                        {
                            th.EntryS = "TH";
                        }
                        ctr++;
                    }

                    foreach (var index in removeSpan)
                    {
                        bodyCollection.EntryK.RemoveAt(index);
                    }
                }
                else
                {
                        if (bodyCollection.EntryK == null)
                            return;
                        if (bodyCollection.EntryK.Count == 0)
                            return;

                    var firstRow = (StructureElementElement)bodyCollection.EntryK[0];


                    foreach (StructureElementElement th in firstRow.EntryK)
                    {
                        if (th.EntryS != null)
                        {
                            if (th.EntryS != "TH")
                            {
                                th.EntryS = "TH";
                            }
                        }
                        else
                        {
                            th.EntryS = "TH";
                        }
                    }
                }
            }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("fixTableHeaderForMultipleRow", ex);

                throw new Exception("fixTableHeaderForMultipleRow", ex);
            }
        }

        private void fixTableSpan(StructureElementElement row)
        {
            try
            { 
            var removeSpan = new List<int>();
            int ctr = 0;
            if (row.EntryK != null)
            {
                if (row.EntryK.Count == 1)
                {
                        var span = ((StructureElementElement)row.EntryK[0]);
                        if (span != null)
                        {
                            if (span.EntryS != null)
                            {
                                if (span.EntryS == "Span")
                                    span.EntryS = "TH";
                            }
                        }
                }
                else
                {
                    foreach (StructureElementElement cell in row.EntryK)
                    {
                            if (cell != null)
                            {
                                if (cell.EntryS != null)
                                {
                                    if (cell.EntryS == "Span")
                                        removeSpan.Add(ctr);
                                }
                            }
                        ctr++;
                    }
                    foreach (var index in removeSpan)
                    {
                        row.EntryK.RemoveAt(index);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("fixTableSpan", ex);

                throw new Exception("fixTableSpan", ex);
            }
        }

        private void fixSingleTableHeader(StructureElementElement table)
        {

            try
            {
                if (table.EntryK.Count == 1)
                {
                    var tbodyOrTR = ((StructureElementElement)table.EntryK[0]);
                    if (tbodyOrTR.EntryS == "TR") // format is : Table/TR/TH
                    {
                        StructureElementElement tHead = new StructureElementElement(table.EntryK);
                        tHead.EntryType = "StructElem";
                        tHead.EntryS = "THead";
                        tHead.EntryK = table.EntryK;
                        tHead.SetParent(table);
                        table.EntryK.RemoveAt(0);

                        StructureElementElement tBody = new StructureElementElement(new IndirectObject());
                        tBody.EntryType = "StructElem";
                        tBody.EntryS = "TBody";
                        tBody.SetParent(table);
                    }


                    if (tbodyOrTR.EntryS == "TBody")// format is : Table/TBody/TR/TH changed to Table/Thead/TR/TH and added new TBody
                    {
                        // tbodyOrTR.EntryS = "THead";

                        //create head
                        //PageObjectElement page = new PageObjectElement(_doc.ObjectSoup[_doc.Page]);
                        StructureElementElement tHead = new StructureElementElement(table.EntryK);
                        tHead.EntryType = "StructElem";
                        tHead.EntryS = "THead";
                        tHead.SetParent(table);


                        //move all the rows to Tbody from Thead accept first row 
                        if (tbodyOrTR.EntryK.Count > 1)
                        {
                            //_logger.Debug("count > 1:" + tbodyOrTR.EntryK.Count);


                          
                            var bodyCollection = (StructureElementElement)table.EntryK[0];
                            if (bodyCollection.EntryK.Count > 0)
                            {
                                var firstRow = (StructureElementElement)bodyCollection.EntryK[0].Clone();
                                //_logger.Debug("first row");

                                foreach (StructureElementElement th in firstRow.EntryK)
                                {
                                    if (th.EntryS != null)
                                    {
                                        if (th.EntryS != "TH")
                                        {
                                            th.EntryS = "TH";
                                        }
                                    }
                                    else
                                    {
                                        th.EntryS = "TH";
                                    }
                                }
                                //_logger.Debug("Add row");

                                tHead.EntryK = new ArrayElement<Element>((Element)firstRow);
                                tHead.EntryK.Add((Element)firstRow);
                                //_logger.Debug("Remove from tbody");

                                bodyCollection.EntryK.RemoveAt(0);
                            }

                            StructureElementElement tbody = null;
                            //swap tbody with THeader
                            foreach (StructureElementElement rows in table.EntryK)
                            {
                                tbody = (StructureElementElement)rows.Clone();
                                break;
                            }
                            if (tbody != null)
                            {
                                //_logger.Debug("swap");

                                table.EntryK.RemoveAt(0);
                                table.EntryK.Add(tbody);
                                //_logger.Debug("donw");
                            }
                            //rename Span to TD
                            if (table.EntryK.Count > 1)
                            {
                                //
                                var newBodyCollection = (StructureElementElement)table.EntryK[1];
                                //_logger.Debug("Tbody collection");

                                foreach (StructureElementElement row in newBodyCollection.EntryK)
                                {

                                    if (row.EntryK == null)
                                        continue;
                                    
                                    foreach (StructureElementElement tr in row.EntryK)
                                    {
                                        
                                        if (tr.EntryS != null)
                                        {
                                             if (tr.EntryS != "TD")
                                            {
                                                tr.EntryS = "TD";
                                            }
                                        }
                                    }
                                }
                            }
                            

                        }
                       
                    }

                }           
                
            }
            catch(Exception ex)
            {
                _logger.ErrorException("fixSingleTableHeader", ex);
                throw new Exception("fixSingleTableHeader", ex);
            }
        }
        public void FixPartToDocTag()
        {
            List<StructureElementElement> elementDoc = FindElementsByType("Document");
            if (elementDoc != null)
            {
                if (elementDoc.Count == 0)
                {
                    List<StructureElementElement> elements = FindElementsByType("Part");

                    if (elements != null)
                    {
                        if (elements.Count > 0)
                        {

                            elements[0].EntryS = "Document";
                        }
                    }
                }
            }
            //if (_doc.GetInfo(_doc.Root, "/Lang") == "")
            //{
            //    _doc.SetInfo(_doc.Root, "/Lang", "(en)");
            //}
            //_doc.SetInfo(_doc.Root, "/MarkInfo", "<< /Marked true /UserProperties false /Suspects false >>");
            //_doc.SetInfo(_doc.Root, "/ViewerPreferences*/DisplayDocTitle", "true");


        }
        public void FixAnnotOnLink()
        {
            List<StructureElementElement> elements = FindElementsByType("Link");
            foreach (StructureElementElement element in elements)
            {

                /* if (element.EntryP.GetType() == typeof(StructureElementElement))
                 {
                     var parent = (StructureElementElement)element.EntryP;

                     if (parent.EntryS == "P" && parent.EntryK.Count == 1 && element.EntryK.Count==1)
                     {
                         var span = (Element) element.EntryK[0].Clone();
                         parent.EntryK.Add(span);
                         parent.EntryK.Remove(element);

                         break;

                     }
                 }*/
                if (element.EntryPg.EntryAnnots != null)
                {
                    foreach (var annot in element.EntryPg.EntryAnnots)
                    {
                        if (annot.EntryContents == null)
                            annot.EntryContents = "link";
                    }
                }
                else
                {

                }
                // break;
                //   element.EntryAlt = "test";
                // element.EntryType = "Annot";
                //  element.EntryT = "link";
            }
        }

        public void FixAnnotOnFigure()
        {

            List<StructureElementElement> elements = FindElementsByType("Figure");
            foreach (StructureElementElement element1 in elements)
            {
                if (element1.EntryAlt == null)
                    element1.EntryAlt = "Figure";

                //     FormXObject o = new FormXObject(element1.Object.Soup);
                //    _doc.SetInfo(element1.Object.ID, "/A/BBox:Rect",x.ToString());
                //
                //      XRect x = new XRect();
                //        x.SetRect(10, 10, 10, 10);
                //     _doc.SetInfo(element1.Object.ID, "/A/BBox:Rect", x.ToString());

                // element1.DictAtom.Add("/A/BBox:Rect", x.ToString());

                /*  var num9 = _doc.AddObject("<< /Type /StructElem /S /Figure /A << /O /Layout /Placement /Block >> >>");
                _doc.SetInfo(num9, "/Alt:Text", element1.EntryAlt);
                _doc.SetInfo(num9, "/A/BBox:Rect", o.BBox.ToString());*/

                //_doc.SetInfo(element1.Object.ID, "/A/BBox:Rect /O /Layout /Placement /Block", x.ToString());



                //  var elemArry = new ArrayElement<StructureElementElement>();

                //PageObjectElement page = new PageObjectElement(_doc.ObjectSoup[_doc.Page]);
                //StructureElementElement tHead = new StructureElementElement(page);
                //tHead.Assign(o.)
                //tHead.SetParent(element1);

                //var item = ((WebSupergoo.ABCpdf11.Atoms.DictAtom)element1.Atom);


                //   doc.SetInfo(num13, "/A[0]/BBox:Rect", value.Rect.ToString(null));
                // "<<\n/BBox [76.5354 162.992 513.071 490.394]\n/O /Layout\n/Placement /Block\n>>"

                if (element1.EntryA == null)
                {
                    XRect x1 = new XRect();
                    x1.SetRect(10, 10, 10, 10);

                    PageObjectElement page = new PageObjectElement(_doc.ObjectSoup[_doc.Page]);
                    var rct = new RectangleElement(page);


                    rct.Elements = new List<double>();

                    rct.Elements.Add(10);
                    rct.Elements.Add(10);
                    rct.Elements.Add(10);
                    rct.Elements.Add(10);

                    var stdLayoutAttributeElment = new StandardLayoutAttributesElement(element1);
                    stdLayoutAttributeElment.EntryO = "Layout";
                    stdLayoutAttributeElment.EntryPlacement = "Block";
                    stdLayoutAttributeElment.EntryBBox = rct;


                    var arry = new ArrayElement<Element>(element1);
                    arry.Add(stdLayoutAttributeElment);

                    element1.EntryA = arry;
                }


                //    element1.EntryA

                /*    FormXObject o = new FormXObject(element1.Object.Soup);

                    Element e = new Element();
                    var rect = _doc.AddObject("/A/BBox:Rect");
                    element1.EntryA = null;

                    var list = new ArrayElement<Element>();
              //      list.Add(rect);

                //    x.SetRect(10, 10, 10, 10);
                //  element1.EntryA = x;

                    if (element1.EntryA==null)
                        _doc.SetInfo(element1.Object.ID, "/A/BBox:Rect", x.ToString());*/
            }

        }
        public void Load(Doc theDoc)
        {
            _doc = theDoc;
            _catalog = theDoc.ObjectSoup.Catalog;
            ScanPages(theDoc);
            Atom root = Atom.GetItem(_catalog.Atom, "StructTreeRoot");
            if (root != null)
            {
                _root = new StructureElementElement(root, _catalog);
                _parentTree = new ParentTree(this);
            }
        }


        public List<StructureElementElement> FindElementsByType(string type)
        {
            List<StructureElementElement> items = new List<StructureElementElement>();
            FindElementsByType(_root, items, type);
            return items;
        }

        private void FindElementsByType(StructureElementElement item, List<StructureElementElement> list, string type)
        {
            if (item == null)
                return;
            if (item.EntryS == type)
                list.Add(item);
            if (item.EntryK != null)
            {
                foreach (Element kid in item.EntryK)
                    if (kid is StructureElementElement)
                        FindElementsByType((StructureElementElement)kid, list, type);
            }
        }

        public List<Element> FindAll(StructureElementElement item)
        {
            List<Element> items = new List<Element>();
            FindAll(item, items);
            return items;
        }

        private void FindAll(StructureElementElement item, List<Element> list)
        {
            if (item == null)
                return;
            list.Add(item);
            if (item.EntryK != null)
            {
                foreach (Element k in item.EntryK)
                {
                    StructureElementElement kid = k as StructureElementElement;
                    if (kid == null)
                        list.Add(k);
                    else
                        FindAll(kid, list);
                }
            }
        }

        public StringBuilder ExtractStructure()
        {
            StringBuilder sb = new StringBuilder();
            ExtractStructure(_root, sb, "");
            return sb;
        }

        private void ExtractStructure(StructureElementElement item, StringBuilder sb, string indent)
        {
            // root isn't really a StructureElement, though for many purposes we can treat it as one
            bool isRoot = item.EntryType == "StructTreeRoot";
            string tagName = isRoot ? "Root" : item.EntryS;
            sb.Append(indent + "<" + tagName);
            if (item.EntryPg != null)
            {
                int number = _pages[item.EntryPg.Object.ID] + 1;
                sb.Append(" PageNumber=\"" + number.ToString() + "\"");
            }
            sb.Append(">\r\n");
            if (item.EntryActualText != null)
            {
                string[] lines = WebUtility.HtmlEncode(item.EntryActualText).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                    sb.Append(indent + "\t" + line + "\r\n");
            }
            if (item.EntryK != null)
            {
                foreach (Element kid in item.EntryK)
                {
                    if (kid == null)
                        continue; // shouldn't happen
                    else if (kid is StructureElementElement)
                    {
                        StructureElementElement se = (StructureElementElement)kid;
                        ExtractStructure(se, sb, indent + "\t");
                    }
                    else if (kid is ObjectReferenceElement)
                    {
                        ObjectReferenceElement or = (ObjectReferenceElement)kid;
                        int id = or.EntryObj.Object != null ? or.EntryObj.Object.ID : 0;
                        string type = or.EntryObj.Object != null ? or.EntryObj.Object.GetType().Name : "Unknown";
                        sb.AppendFormat("{0}\t<ObjectReference ID=\"{1}\" Type=\"{2}\" />\r\n", indent, id, type);
                    }
                    else if (kid is MarkedContentReferenceElement)
                    {
                        MarkedContentReferenceElement mcr = (MarkedContentReferenceElement)kid;
                        sb.AppendFormat("{0}\t<MarkedContentReference MCID=\"{1}\" />\r\n", indent, mcr.EntryMCID.Value.ToString());
                    }
                    else if (kid.NumAtom != null)
                    {
                        int mcid = kid.NumAtom.Num;
                        sb.AppendFormat("{0}\t<MCID ID=\"{1}\" />\r\n", indent, mcid.ToString());
                    }
                }
            }
            sb.Append(indent + "</" + tagName + ">\r\n");
        }

        public void MarkupStructure()
        {
            _doc.TextStyle.Size = 3;
            _doc.Width = 0.2;
            MarkupStructure(_root, 0, 0);
        }

        private Tuple<int, XRect> MarkupStructure(StructureElementElement item, int level, int index)
        {
            // root isn't really a StructureElement, though for many purposes we can treat it as one
            bool isRoot = item.EntryType == "StructTreeRoot";
            string tagName = isRoot ? "Root" : item.EntryS;
            int pageID = 0;
            XRect bbox = null;
            if (item.EntryPg != null)
            {
                pageID = item.EntryPg.Object.ID;
                _doc.Page = pageID;
                bbox = item.GetBBox();
                if (bbox != null)
                {
                    SetDistinctColor(level);
                    _doc.Rect.String = bbox.String;
                    _doc.FrameRect();
                    _doc.AddText(String.Format("Tag: {0} Level: {1} Item: {2}", tagName, level, index));
                }
            }
            if (item.EntryK != null)
            {
                Tuple<int, XRect> previous = null;
                for (int i = 0; i < item.EntryK.Count; i++)
                {
                    Tuple<int, XRect> current = null;
                    Element kid = item.EntryK[i];
                    if (kid == null)
                        continue; // shouldn't happen
                    else if (kid is StructureElementElement)
                    {
                        StructureElementElement se = (StructureElementElement)kid;
                        current = MarkupStructure(se, level + 1, i);
                    }
                    // see ExtractStructure for other possible object types
                    if ((previous != null) && (current != null))
                    {
                        if (previous.Item1 != current.Item1)
                        { // page change
                            _doc.Page = previous.Item1;
                            _doc.AddLine(previous.Item2.Right, previous.Item2.Bottom, _doc.CropBox.Right, _doc.CropBox.Bottom, LineEnding.None, LineEnding.ClosedArrow, 5, 5);
                            _doc.Page = current.Item1;
                            _doc.AddLine(_doc.CropBox.Left, _doc.CropBox.Top, current.Item2.Left, current.Item2.Top, LineEnding.None, LineEnding.ClosedArrow, 5, 5);
                        }
                        else
                        {
                            double x1 = previous.Item2.Right < current.Item2.Left ? previous.Item2.Right : previous.Item2.Left;
                            double x2 = previous.Item2.Right < current.Item2.Left ? current.Item2.Left : current.Item2.Right;
                            double y1 = previous.Item2.Bottom > current.Item2.Top ? previous.Item2.Bottom : previous.Item2.Top;
                            double y2 = previous.Item2.Bottom > current.Item2.Top ? current.Item2.Top : current.Item2.Bottom;
                            _doc.AddLine(x1, y1, x2, y2, LineEnding.None, LineEnding.ClosedArrow, 5, 5);
                        }
                    }
                    previous = current;
                }
            }
            return bbox != null ? new Tuple<int, XRect>(pageID, bbox) : null;
        }

        private void SetDistinctColor(int i)
        {
            int divisor = (i / 6) % 8;
            int value = (2 << (7 - divisor)) - 1;
            switch (i % 6)
            {
                case 0: _doc.Color.SetRgb(0, 0, value); break;
                case 1: _doc.Color.SetRgb(0, value, 0); break;
                case 2: _doc.Color.SetRgb(value, 0, 0); break;
                case 3: _doc.Color.SetRgb(0, value, value); break;
                case 4: _doc.Color.SetRgb(value, 0, value); break;
                case 5: _doc.Color.SetRgb(value, value, 0); break;
            }
        }

        public void ReplaceImages(List<StructureElementElement> images, PixMap pm)
        {
            Dictionary<Page, HashSet<int>> pages = GetMCIDsByPage(images);
            foreach (KeyValuePair<Page, HashSet<int>> pair in pages)
            {
                Page page = pair.Key;
                StreamObject layer = null;
                ArrayAtom array = null;
                Dictionary<int, KeyValuePair<int, int>> mcids;
                FindMCIDs(pair.Key, out layer, out array, out mcids);
                string imageName = page.AddResource(pm, "XObject", "ABCpdf" + pm.ID);
                HashSet<string> names = new HashSet<string>();
                foreach (int id in pair.Value)
                {
                    KeyValuePair<int, int> bounds;
                    if (mcids.TryGetValue(id, out bounds))
                    {
                        for (int i = bounds.Key; i <= bounds.Value; i++)
                        {
                            OpAtom op = array[i] as OpAtom;
                            if (op == null) continue;
                            if (op.Text != "Do") continue;
                            if (i == 0) continue;
                            NameAtom name = array[i - 1] as NameAtom;
                            if (name == null) continue;
                            names.Add(name.Text);
                            name.Text = imageName;
                        }
                    }
                }
                byte[] arrayData = array.GetData();
                layer.SetData(arrayData, 1, arrayData.Length - 2);
                layer.CompressFlate();
                OpAtom.Find(array, new string[] { "Do" });
                IList<Tuple<string, int>> items = OpAtom.Find(array, new string[] { "Do" });
                for (int i = 0; i < items.Count; i++)
                {
                    int pos = items[i].Item2 - 1;
                    if (pos < 0) continue;
                    string name = Atom.GetName(array[pos]);
                    if (name == null) continue;
                    names.Remove(name);
                }
                foreach (string name in names)
                    Utilities.RemoveResource(page, "XObject", name);
            }
        }

        private Dictionary<Page, HashSet<int>> GetMCIDsByPage(List<StructureElementElement> elements)
        {
            Dictionary<Page, HashSet<int>> pages = new Dictionary<Page, HashSet<int>>();
            foreach (StructureElementElement parent in elements)
            {
                Page page = parent.GetPage(this);
                List<Element> items = FindAll(parent);
                foreach (Element item in items)
                {
                    if ((page == null) && (item is StructureElementElement))
                        page = ((StructureElementElement)item).GetPage(this); // this needs adjustment for possibility structure may span more than one page
                    if (page == null)
                        continue;
                    int mcid = (item.Atom is NumAtom) ? ((NumAtom)item.Atom).Num : -1;
                    if (mcid < 0)
                        continue;
                    HashSet<int> mcids = null;
                    if (!pages.TryGetValue(page, out mcids))
                    {
                        mcids = new HashSet<int>();
                        pages[page] = mcids;
                    }
                    mcids.Add(mcid);
                }
            }
            return pages;
        }

        public void ReplaceText(List<StructureElementElement> items, string text)
        {
            Dictionary<Page, HashSet<int>> pages = GetMCIDsByPage(items);
            foreach (KeyValuePair<Page, HashSet<int>> pair in pages)
            {
                Page page = pair.Key;
                StreamObject layer = null;
                ArrayAtom array = null;
                Dictionary<int, KeyValuePair<int, int>> mcids;
                FindMCIDs(pair.Key, out layer, out array, out mcids);
                Dictionary<string, string> fontNames = new Dictionary<string, string>();
                IDictionary<string, Atom> fontMap = page.GetResourceMap(page.ID, "Font");
                foreach (int id in pair.Value)
                {
                    KeyValuePair<int, int> bounds;
                    if (mcids.TryGetValue(id, out bounds))
                    {
                        for (int i = bounds.Key; i <= bounds.Value; i++)
                        {
                            OpAtom op = array[i] as OpAtom;
                            if (op == null) continue;
                            if (i == 0) continue; // shouldn't happen
                            StringAtom stringToChange = null;
                            if (op.Text == "TJ")
                            {
                                ArrayAtom parameters = array[i - 1] as ArrayAtom;
                                int n = parameters != null ? parameters.Count : 0;
                                for (int j = 0; j < n; j++)
                                {
                                    Atom parameter = parameters[j];
                                    if (parameter is NumAtom)
                                        ((NumAtom)parameter).Num = 0;
                                    else if (parameter is StringAtom)
                                    {
                                        stringToChange = (StringAtom)parameter;
                                        stringToChange.Text = "";
                                    }
                                }
                            }
                            else if ((op.Text == "Tj") || (op.Text == "\"") || (op.Text == "\'"))
                            {
                                stringToChange = array[i - 1] as StringAtom;
                            }
                            if (stringToChange != null)
                            {
                                NameAtom fontName = null;
                                for (int j = i - 2; j >= 2; j--)
                                {
                                    op = array[j] as OpAtom;
                                    if ((op != null) && (op.Text == "Tf"))
                                    {
                                        fontName = array[j - 2] as NameAtom;
                                        if (fontName != null)
                                            break;
                                    }
                                }
                                if (fontName != null)
                                {
                                    string oldName = fontName.Text;
                                    if (!fontNames.ContainsKey(oldName))
                                    {
                                        string newName = null;
                                        Atom fontRez = null;
                                        if (fontMap.TryGetValue(oldName, out fontRez))
                                        {
                                            FontObject fontObj = page.ResolveObj(fontRez) as FontObject;
                                            if (fontObj != null)
                                            {
                                                if (!fontObj.IsSubset)
                                                {
                                                    newName = oldName;
                                                }
                                                else
                                                { // need to replace font - though we could check if it contains the right characters first
                                                    int fontID = _doc.AddFont(fontObj.BaseFont, LanguageType.Latin, false);
                                                    if (fontID != 0)
                                                        newName = page.AddResource(_doc.ObjectSoup[fontID], "Font", "ABCfont" + fontID.ToString());
                                                }
                                            }
                                        }
                                        fontNames[oldName] = newName;
                                    }
                                    if (fontNames[oldName] != null)
                                    {
                                        if (oldName != fontNames[oldName])
                                            fontName.Text = fontNames[oldName];
                                        stringToChange.Text = text;
                                    }
                                }
                            }
                        }
                    }
                }
                byte[] arrayData = array.GetData();
                layer.SetData(arrayData, 1, arrayData.Length - 2);
                layer.CompressFlate();
            }
        }

        private void FindMCIDs(Page page, out StreamObject layer, out ArrayAtom array, out Dictionary<int, KeyValuePair<int, int>> mcids)
        {
            mcids = new Dictionary<int, KeyValuePair<int, int>>();
            page.DeInline(false);
            page.Flatten(true, false);
            StreamObject[] contents = page.GetLayers();
            Debug.Assert(contents.Length == 1);
            layer = contents[0];
            layer.Decompress();
            array = ArrayAtom.FromContentStream(layer.GetText());
            IList<Tuple<string, int>> items = OpAtom.Find(array, new string[] { "BDC", "EMC" });
            for (int i = 0; i < items.Count; i++)
            {
                Tuple<string, int> item1 = items[i];
                if (item1.Item1 == "BDC")
                {
                    DictAtom dict = item1.Item2 > 0 ? array[item1.Item2 - 1] as DictAtom : null;
                    if (dict == null) continue;
                    NumAtom mcid = Atom.GetItem(dict, "MCID") as NumAtom;
                    if (mcid == null) continue;
                    int depth = 1;
                    for (int j = i + 1; j < items.Count; j++)
                    {
                        Tuple<string, int> item2 = items[j];
                        if (item2.Item1 == "BDC")
                            depth++;
                        else if (item2.Item1 == "EMC")
                            depth--;
                        if (depth == 0)
                        {
                            mcids[mcid.Num] = new KeyValuePair<int, int>(item1.Item2, item2.Item2);
                            break;
                        }
                    }
                }
            }
        }

        private void ScanPages(Doc theDoc)
        {
            _pages = new Dictionary<int, int>();
            _contents = new string[theDoc.PageCount];
            _layers = new List<StreamObject>();
            for (int i = 0; i < theDoc.PageCount; i++)
            {
                theDoc.PageNumber = i + 1;
                Page page = theDoc.ObjectSoup[theDoc.Page] as Page;
                StringBuilder sb = new StringBuilder();
                StreamObject[] contents = page.GetLayers();
                for (int j = 0; j < contents.Length; j++)
                {
                    contents[j].Decompress();
                    sb.Append(contents[j].GetText());
                    contents[j].CompressFlate();
                    _layers.Add(contents[j]);
                }
                _pages[page.ID] = i;
                _contents[i] = sb.ToString();
            }
        }

        public bool AssignPagesToStructItems()
        {
            if (_parentTree.AssignPagesToStructItems())
            {
                GetPageFromKids(_root, 0);
                return true;
            }
            return false;
        }

        private Page GetPageFromKids(StructureElementElement item, int depth)
        {
            Page page = item.GetPage();
            if (page != null)
                return page;
            if ((page == null) && (item.EntryK != null))
            {
                int n = item.EntryK.Count;
                for (int i = 0; i < n; i++)
                {
                    StructureElementElement element = item.EntryK[i] as StructureElementElement;
                    if (element == null)
                        continue;
                    Page kidPage = GetPageFromKids(element, depth + 1);
                    if (i == 0)
                    {
                        page = kidPage;
                        continue;
                    }
                    if (kidPage.ID != page.ID)
                        return null;
                }
                if ((page != null) && (depth != 0))
                {
                    item.SetPage(page);
                    for (int i = 0; i < n; i++)
                    {
                        StructureElementElement element = item.EntryK[i] as StructureElementElement;
                        if (element != null)
                            element.EntryPg = null;
                    }
                }
            }
            return page;
        }

        public int GetPageNumber(Page page)
        {
            return GetPageNumber(page.ID);
        }
        public int GetPageNumber(int page)
        {
            return _pages[page] + 1;
        }
    }

    public class KidArranger
    {
        public static void SortTopToBottom(Structure structure, StructureElementElement parent)
        {
            KidArranger list = new KidArranger(structure, parent.EntryK);
            list.SortTopToBottom();
            list.Commit(parent.EntryK);
        }

        public static void SortLeftToRight(Structure structure, StructureElementElement parent)
        {
            KidArranger list = new KidArranger(structure, parent.EntryK);
            list.SortLeftToRight();
            list.Commit(parent.EntryK);
        }

        [DebuggerDisplay("\\{ Index = {Index} Page = {PageNumber} BBox = {BBox} \\}")]
        internal class Position
        {
            public int Index { get; set; }
            public StructureElementElement Element { get; set; }
            public int PageNumber { get; set; }
            public XRect BBox { get; set; }
            public Position(Structure structure, int index, StructureElementElement element)
            {
                Index = index;
                Element = element;
                if (element != null)
                {
                    if (element.GetPage() != null)
                    {
                        PageNumber = structure.GetPageNumber(element.GetPage());
                        BBox = element.GetBBox();
                        if (element.EntryA != null)
                        {

                            foreach (var attribute in element.EntryA)
                            {
                                if (attribute is StandardLayoutAttributesElement == false)
                                    continue;
                                if (((StandardLayoutAttributesElement)attribute).EntryBBox != null)
                                {

                                    var bbox = ((StandardLayoutAttributesElement)attribute).EntryBBox.Elements;
                                    if (bbox != null)
                                    {
                                        BBox = XRect.FromNums(bbox);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
                if (BBox == null)
                    BBox = new XRect();
            }
        };

        private List<Position> _items = null;

        private KidArranger() { }
        private KidArranger(Structure structure, ArrayElement<Element> items)
        {
            _items = new List<Position>();
            int index = 0;
            foreach (Element item in items)
                _items.Add(new Position(structure, index++, item as StructureElementElement));
        }

        private void Commit(ArrayElement<Element> items)
        {
            Element[] copy = new Element[items.Count];
            items.CopyTo(copy, 0);
            ArrayAtom array = items.ArrayAtom;
            array.Clear();
            foreach (Position pos in _items)
            {
                Element e = copy[pos.Index];
                array.Add(e.RefOrAtom);
            }
        }

        private void SortTopToBottom()
        {
            // ... from low pages to high pages
            // ... or if equal, from top to bottom
            // ... or if equal, from left to right
            _items.Sort((i1, i2) => SortTopToBottom(i1, i2));
        }

        private void SortLeftToRight()
        {
            // ... from low pages to high pages
            // ... or if equal, from left to right
            // ... or if equal, from top to bottom
            _items.Sort((i1, i2) => SortLeftToRight(i1, i2));
        }

        private static int SortTopToBottom(Position p1, Position p2)
        {
            const double epsilon = 3.0; // vertical resolution in points
            if (p1.PageNumber != p2.PageNumber)
                return p1.PageNumber.CompareTo(p2.PageNumber);
            if (Math.Abs(p1.BBox.Top - p2.BBox.Top) > epsilon)
                return p2.BBox.Top.CompareTo(p1.BBox.Top);
            return p1.BBox.Left.CompareTo(p2.BBox.Left);
        }

        private static int SortLeftToRight(Position p1, Position p2)
        {
            const double epsilon = 3.0; // horizontal resolution in points
            if (p1.PageNumber != p2.PageNumber)
                return p1.PageNumber.CompareTo(p2.PageNumber);
            if (Math.Abs(p1.BBox.Left - p2.BBox.Left) > epsilon)
                return p2.BBox.Left.CompareTo(p1.BBox.Left);
            return p1.BBox.Top.CompareTo(p2.BBox.Top);
        }
    }

    public class ParentTree
    {
        private Structure _structure = null;
        private DictAtom _dict;
        private Dictionary<int, Atom> _tree = null;
        private int _treeNextKey = -1;
        private Dictionary<int, Tuple<IndirectObject, bool>> _parents = null;
        private bool _pagesAssigned = false;

        private ParentTree() { }
        public ParentTree(Structure structure)
        {
            _structure = structure;
            _dict = _structure.Catalog.Resolve(Atom.GetItem(_structure.Root.Atom, "ParentTree")) as DictAtom;
        }

        public DictAtom DictAtom { get { return _dict; } }

        public bool AssignPagesToStructItems()
        {
            if (_pagesAssigned)
                return false;
            Load();
            foreach (var pair in _parents)
            {
                int parentStructID = pair.Key;
                IndirectObject parent = pair.Value.Item1;
                bool hasManyEntries = pair.Value.Item2;
                Debug.Assert(_tree.ContainsKey(parentStructID));
                if ((hasManyEntries) && (parent is Page))
                { // StructParents -> more than one MCID
                    if (parent is Page)
                    {
                        ArrayAtom array = _structure.Catalog.Resolve(_tree[parentStructID]) as ArrayAtom;
                        Debug.Assert(array != null);
                        int n = array != null ? array.Count : 0;
                        for (int i = 0; i < n; i++)
                        {
                            Atom kid = _structure.Catalog.Resolve(array[i]);
                            Atom.SetItem(kid, "Pg", new RefAtom(parent));
                        }
                    }
                }
                else
                {  // StructParent -> individual content item
                    if (parent is Annotation)
                    {
                        Page page = ((Annotation)parent).Page;
                        Atom kid = _structure.Catalog.Resolve(_tree[parentStructID]);
                        Debug.Assert(page != null);
                        Atom.SetItem(kid, "Pg", new RefAtom(page));
                    }
                }
            }
            _pagesAssigned = true;
            return true;
        }

        public int AddStructParent(IndirectObject io, bool commit = true)
        {
            return AddStructParentOrParents(io, false, commit);
        }

        public int AddStructParents(IndirectObject io, bool commit = true)
        {
            return AddStructParentOrParents(io, true, commit);
        }

        private int AddStructParentOrParents(IndirectObject io, bool isParentNotParents, bool commit)
        {
            Load();
            _tree.Add(_treeNextKey, new RefAtom(io));
            _parents[_treeNextKey] = new Tuple<IndirectObject, bool>(io, isParentNotParents);
            if (commit)
            {
                IndirectObject tree = Utilities.SaveNumberTree(_tree, _structure.Catalog.Soup);
                Atom.SetItem(_structure.Root.Atom, "ParentTree", new RefAtom(tree));
                Atom.SetItem(_structure.Root.Atom, "ParentTreeNextKey", new NumAtom(_treeNextKey + 1));
            }
            return _treeNextKey++;
        }

        private void Load()
        {
            if (_tree != null)
                return;
            _tree = Utilities.LoadNumberTree(_structure.Catalog, _dict);
            _treeNextKey = -1;
            foreach (var pair in _tree)
                _treeNextKey = Math.Max(_treeNextKey, pair.Key);
            _treeNextKey++;
            _parents = FindAllParents();
        }

        private Dictionary<int, Tuple<IndirectObject, bool>> FindAllParents()
        {
            Dictionary<int, Tuple<IndirectObject, bool>> parents = new Dictionary<int, Tuple<IndirectObject, bool>>();
            foreach (Page page in _structure.Catalog.Pages.GetPageArrayAll())
            {
                AddIfParent(page, parents);
                foreach (Annotation annot in page.GetAnnotations())
                    AddIfParent(annot, parents);
                ISet<Atom> rez = page.GetResourcesByType("XObject", true, true, true, true, null);
                foreach (Atom atom in rez)
                    AddIfParent(_structure.Catalog.ResolveObj(atom), parents);
            }
            return parents;
        }

        private void AddIfParent(IndirectObject io, Dictionary<int, Tuple<IndirectObject, bool>> parents)
        {
            if (io != null)
            {
                NumAtom structParent = _structure.Catalog.Resolve(Atom.GetItem(io.Atom, "StructParent")) as NumAtom;
                if (structParent != null)
                    parents[structParent.Num] = new Tuple<IndirectObject, bool>(io, false);
                NumAtom structParents = _structure.Catalog.Resolve(Atom.GetItem(io.Atom, "StructParents")) as NumAtom;
                if (structParents != null)
                    parents[structParents.Num] = new Tuple<IndirectObject, bool>(io, true);
                Debug.Assert((structParent == null) || (structParents == null));
            }
        }
    };

    public static class Extension
    {
        public static XRect GetBBox(this StructureElementElement element)
        {
            if (element.EntryA != null)
            {
                foreach (var attribute in element.EntryA)
                {

                    if (attribute is StandardLayoutAttributesElement == false)
                        continue;
                    if (((StandardLayoutAttributesElement)attribute).EntryBBox != null)
                    {
                        var bbox = ((StandardLayoutAttributesElement)attribute).EntryBBox.Elements;
                        if (bbox != null)
                            return XRect.FromNums(bbox);
                    }
                }
            }
            return new XRect();
        }

        public static Page GetPage(this StructureElementElement element)
        {
            PageObjectElement io = element.EntryPg;
            if (io == null) return null;
            Debug.Assert(io.Object is Page);
            return io.Object as Page;
        }

        public static Page GetPage(this StructureElementElement element, Structure structure)
        {
            StructureElementElement current = element;
            while (current != null)
            {
                Page page = current.GetPage();
                if (page != null)
                    return page;
                current = current.EntryP as StructureElementElement;
            }
            if (structure.AssignPagesToStructItems())
                return element.GetPage(structure);
            return null;
        }

        public static void SetPage(this StructureElementElement element, Page page)
        {
            element.EntryPg = page != null ? new PageObjectElement(page) : null;
        }

        public static Dictionary<string, string> GetStyle(this StructureElementElement element)
        {
            StringAtom atom = element.DictAtom != null ? element.Host.Resolve(element.DictAtom["XXStyle"]) as StringAtom : null;
            return atom != null ? Utilities.ParseStyle(atom.Text) : null;
        }

        public static void SetParent(this StructureElementElement element, StructureElementElement parent)
        {
            int count = parent.EntryK != null ? parent.EntryK.Count : 0;
            SetParent(element, parent, count);
        }

        public static void SetParent(this StructureElementElement element, StructureElementElement parent, int index)
        {
            Debug.Assert(element.EntryP is StructureTreeRootElement == false);
            StructureElementElement oldParent = element.EntryP as StructureElementElement;
            if (oldParent != null)
            {
                bool removed = oldParent.EntryK.Remove(element);
                Debug.Assert(removed);
            }
            if (parent != null)
            {
                if (parent.EntryK == null)
                    parent.EntryK = new ArrayElement<Element>(parent);
                if (index > parent.EntryK.Count)
                    index = parent.EntryK.Count;
                parent.EntryK.Insert(index, element);
            }
            element.EntryP = parent;
        }

        public static void GetMcids(this StructureElementElement element, Dictionary<Page, HashSet<int>> mcids)
        {
            if (element.EntryK == null)
                return;
            Page page = null;
            foreach (Element item in element.EntryK)
            {
                if (item == null)
                    continue; // shouldn't happen
                StructureElementElement kid = item as StructureElementElement;
                if (item.NumAtom != null)
                {
                    if (page == null)
                        page = element.EntryPg.Object as Page;
                    HashSet<int> ids = null;
                    mcids.TryGetValue(page, out ids);
                    if (ids == null)
                    {
                        ids = new HashSet<int>();
                        mcids[page] = ids;
                    }
                    ids.Add(item.NumAtom.Num);
                }
                else if (kid is StructureElementElement)
                {
                    ((StructureElementElement)kid).GetMcids(mcids);
                }
            }
        }
    }

    class Utilities
    {
        public static void RemoveResource(Page page, string type, string name)
        {
            Atom resources = page.Resolve(Atom.GetItem(page.Atom, "Resources"));
            Atom items = page.Resolve(Atom.GetItem(resources, type));
            Atom.RemoveItem(items, name);
        }

        public static Dictionary<int, Atom> LoadNumberTree(IndirectObject io, DictAtom root)
        {
            Dictionary<int, Atom> values = new Dictionary<int, Atom>();
            LoadNumberTree(io, root, values);
            return values;
        }

        private static void LoadNumberTree(IndirectObject io, DictAtom root, Dictionary<int, Atom> values)
        {
            if (root != null)
            {
                ArrayAtom kids = io.Resolve(Atom.GetItem(root, "Kids")) as ArrayAtom;
                if (kids != null)
                {
                    foreach (Atom kid in kids)
                        LoadNumberTree(io, kid as DictAtom, values);
                }
                ArrayAtom nums = io.Resolve(Atom.GetItem(root, "Nums")) as ArrayAtom;
                int n = nums != null ? nums.Count : 0;
                for (int i = 0; i < n; i += 2)
                {
                    NumAtom num = nums[i] as NumAtom;
                    values[num.Num] = nums[i + 1];
                }
            }
        }

        public static IndirectObject SaveNumberTree(Dictionary<int, Atom> tree, ObjectSoup soup)
        {
            IndirectObject io = IndirectObject.FromString("<< /Nums [] >>");
            soup.Add(io);
            ArrayAtom array = Atom.GetItem(io.Atom, "Nums") as ArrayAtom;
            List<int> keys = new List<int>(tree.Keys);
            keys.Sort();
            foreach (int num in keys)
            {
                array.Add(num);
                array.Add(tree[num]);
            }
            return io;
        }

        public static Dictionary<string, string> ParseStyle(string style)
        {
            // eg font-size: 5.4288pt; font-family: Helvetica; line-height: 116pt; color: rgb(102,102,102); )
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] items = style.Split(new char[] { ';' });
            foreach (string item in items)
            {
                int p = item.IndexOf(':');
                if (p > 0)
                {
                    string item1 = item.Substring(0, p).Trim().ToLowerInvariant();
                    string item2 = item.Substring(p + 1).Trim();
                    dict[item1] = item2;
                }
            }
            return dict;
        }
    }
}
