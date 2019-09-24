using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.rulecompiler
{
    public class SourceMergeListCompiler
    {

        Sourcemergelist _sourceMergeList;
        SourceMereList _compiledsourceMergeResult;

        public SourceMergeListCompiler(Sourcemergelist sourceMergeList)
        {
            _sourceMergeList = sourceMergeList;
            _compiledsourceMergeResult = new SourceMereList();
        }

        public SourceMereList GetCompiledSourceMergeList()
        {
            SourceMergeActionCompiler actionCompiler = new SourceMergeActionCompiler(_sourceMergeList.sourcemergeactions);
            _compiledsourceMergeResult.SourceMergeActions = actionCompiler.GetSourceMergeActions();

            return _compiledsourceMergeResult;
        }
    }
}
