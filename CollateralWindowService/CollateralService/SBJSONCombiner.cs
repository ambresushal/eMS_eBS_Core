using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CollateralService
{
    public class SBJSONCombiner
    {
        string _primaryJSON;
        List<string> _otherJSONs;
        string regexHTMLComment = @"<!--[\sa-zA-Z0-9\[\].\-,'\(\)_]*-->";
        string regexSquareBrackets = @"\[[[\sa-zA-Z0-9.\-,'\(\)_]*]]";

        public SBJSONCombiner(string primaryJSON, List<string> otherJSONs)
        {
            _primaryJSON = primaryJSON;
            _otherJSONs = otherJSONs;
        }

        public string Combine()
        {
            //get html placeholders from other json's
            foreach (string json in _otherJSONs)
            {
                List<Capture> captures = new List<Capture>();
                var matches = Regex.Matches(json, regexHTMLComment);
                foreach (var match in matches)
                {
                    captures.Add(((Match)match).Captures[0]);
                }
                var groupedCaptures = captures.GroupBy(a => a.Value);
                foreach (var groupCapture in groupedCaptures)
                {
                    var cap = groupCapture.ToList();
                    cap = cap.OrderBy(a => a.Index).ToList();
                    //generate the primaryJSON search
                    for (int idx = 0; idx < cap.Count; idx++)
                    {
                        string searchVal = cap[idx].Value.Replace("<!--", "").Replace("-->", "");
                        searchVal = "[[" + searchVal + "]]";
                        if (idx < cap.Count - 1)
                        {
                            int startIndex = cap[idx].Index + cap[idx].Length;
                            int endIndex = cap[idx + 1].Index;
                            string replaceStr = json.Substring(startIndex, endIndex - startIndex);
                            idx = idx + 1;
                            int idxPrimary = _primaryJSON.IndexOf(searchVal);
                            if (idxPrimary >= 0)
                            {
                                _primaryJSON = _primaryJSON.Remove(idxPrimary, searchVal.Length);
                                _primaryJSON = _primaryJSON.Insert(idxPrimary, replaceStr);
                            }
                        }
                    }
                }
            }
            _primaryJSON = Regex.Replace(_primaryJSON, regexHTMLComment, "");
            _primaryJSON = Regex.Replace(_primaryJSON, regexSquareBrackets, "");
            return _primaryJSON;
        }
    }
}
