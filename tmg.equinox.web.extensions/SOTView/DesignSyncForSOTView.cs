using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.SOTView
{
    public class DesignSyncForSOTView
    {
        FormDesignVersionDetail _sourceDetail;
        FormDesignVersionDetail _targetDetail;
        public DesignSyncForSOTView(FormDesignVersionDetail sourceDetail, FormDesignVersionDetail targetDetail)
        {
            _sourceDetail = sourceDetail;
            _targetDetail = targetDetail;
        }

        private void SetItemstoElement(SectionDesign target, SectionDesign source)
        {
            if (source != null && source.Elements != null && source.Elements.Count > 0)
            {
                foreach (ElementDesign targetelement in target.Elements)
                {
                    ElementDesign sourceelement = source.Elements.Where(e => e.Name == targetelement.Name).FirstOrDefault();

                    if (sourceelement != null && sourceelement.Section != null && targetelement.Section != null)
                        SetItemstoElement(targetelement.Section, sourceelement.Section);
                    else
                    {
                        if (sourceelement != null && sourceelement.Items != null && sourceelement.Items.Count > 0)
                            targetelement.Items = sourceelement.Items;
                    }
                }
            }
        }

        public FormDesignVersionDetail GetUpdatedTargetDetail()
        {
            List<SectionDesign> sections = _targetDetail.Sections;
            foreach (SectionDesign target in _targetDetail.Sections)
            {
                SectionDesign source = _sourceDetail.Sections.Where(s => s.Name == target.Name).FirstOrDefault();
                SetItemstoElement(target, source);
            }

            _targetDetail.Sections = sections;
            return _targetDetail;
        }
    }
}
