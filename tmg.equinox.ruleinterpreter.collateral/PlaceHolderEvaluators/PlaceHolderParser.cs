using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class PlaceHolderParser
    {
        string _placeHolder;

        public PlaceHolderParser(string placeHolder)
        {
            _placeHolder = placeHolder;
        }

        public void SetPlaceHolder(string placeHolder)
        {
            _placeHolder = placeHolder;
        }

        public bool HasComplexFunction()
        {
            bool result = false;
            MatchCollection collFunction = Regex.Matches(_placeHolder, RegexConstants.MultipleComplexFunctionRegex);
            if(collFunction.Count > 0)
            {
                result = true;
            }
            else
            {
                collFunction = Regex.Matches(_placeHolder, RegexConstants.ComplexFunctionRegex);
                if (collFunction.Count > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        public bool HasSimpleFunction()
        {
            bool result = false;
            MatchCollection collFunction = Regex.Matches(_placeHolder, RegexConstants.SimpleFunctionRegex);
            if (collFunction.Count > 0)
            {
                result = true;
            }
            return result;
        }
        public bool HasAlias()
        {
            bool result = false;
            MatchCollection collFunction = Regex.Matches(_placeHolder, RegexConstants.AliasRegex);
            if (collFunction.Count > 0)
            {
                result = true;
            }
            return result;
        }
        public bool HasField()
        {
            bool result = false;
            MatchCollection collFunction = Regex.Matches(_placeHolder, RegexConstants.FieldRegex);
            if (collFunction.Count > 0)
            {
                result = true;
            }
            return result;
        }
        public bool HasInternal()
        {
            bool result = false;
            MatchCollection collFunction = Regex.Matches(_placeHolder, RegexConstants.InternalRegex);
            if (collFunction.Count > 0)
            {
                result = true;
            }
            return result;
        }
    }
}
