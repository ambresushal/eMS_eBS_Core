using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class PlaceHolderProcessor
    {
        string _richText;
        List<JToken> _internalContext;
        Dictionary<string, JToken> _sources;
        List<LanguageFormats> _langFormats;
        public PlaceHolderProcessor(string richText, List<JToken> internalContext, Dictionary<string, JToken> sources)
        {
            _richText = richText;
            _internalContext = internalContext;
            _sources = sources;
        }

        public string Process()
        {
            _langFormats = EvaluateLanguageFormats();
            //find and process complex functions
            PlaceHolderParser parser = new PlaceHolderParser(_richText);
            //find and process aliases
            bool hasAlias = parser.HasAlias();
            if (hasAlias == true)
            {
                //process aliases
                AliasPlaceHolderEvaluator evaluator = new AliasPlaceHolderEvaluator(_richText, _sources);
                _richText = evaluator.Evaluate();
                parser.SetPlaceHolder(_richText);
            }
            //find and process fields
            bool hasField = parser.HasField();
            if (hasField == true)
            {
                //process fields
                FieldPlaceHolderEvaluator evaluator = new FieldPlaceHolderEvaluator(_richText, _sources);
                _richText = evaluator.Evaluate();
                parser.SetPlaceHolder(_richText);
            }
            //find and process internal fields
            bool hasInternalField = parser.HasInternal();
            if (hasInternalField == true)
            {
                //process internal fields
                InternalPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(_richText, _internalContext);
                _richText = evaluator.Evaluate();
            }
            //find and process simple functions
            bool hasSimpleFunction = parser.HasSimpleFunction();
            if (hasSimpleFunction == true)
            {
                //process simple functions
                SimpleFunctionPlaceHolderEvaluator evaluator = new SimpleFunctionPlaceHolderEvaluator(_richText, _internalContext, _sources);
                _richText = evaluator.Evaluate();
                parser.SetPlaceHolder(_richText);
            }
            bool hasComplexFunction = parser.HasComplexFunction();
            if(hasComplexFunction == true)
            {
                //process complex functions
                ComplexFunctionPlaceHolderEvaluator evaluator = new ComplexFunctionPlaceHolderEvaluator(_richText, _internalContext, _sources);
                _richText = evaluator.Evaluate();
                parser.SetPlaceHolder(_richText);
            }
            
            
            return _richText;
        }


        private List<LanguageFormats> EvaluateLanguageFormats()
        {
            string formatType = string.Empty;
            List<LanguageFormats> langFormats = new List<LanguageFormats>();
            JToken languageFormatSource = _sources.Where(whr => whr.Key.Contains("FORMAT")).Select(sel => sel.Value).FirstOrDefault();

            if (languageFormatSource != null && languageFormatSource.Count() > 0)
            {
                formatType = languageFormatSource.First().SelectToken(LanguageFormatConstants.FormTypeTokenPath).ToString();
            }
            if (!string.IsNullOrEmpty(formatType))
            {
                LanguageFormatParser parser = new LanguageFormatParser(languageFormatSource);
                langFormats = parser.GetLanguageFormats();
            }
            return langFormats;
        }
    }
}
