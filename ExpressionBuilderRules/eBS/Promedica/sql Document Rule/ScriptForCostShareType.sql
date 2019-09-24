update ML.ElementDocumentRule set RuleJSON=N'{
  "documentrule": {
    "targetelement": "CommercialMedicalAnchor[CascadingCostShare.CostShareGroup.CostShareGroupList]",
    "ruletype": "datasource",
    "targetelementtype": "field",
    "ruleconditions": {
      "sources": [
        {
          "sourcename": "A",
          "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Coinsurance.CoinsuranceList]",
          "sourceelementtype": "repeater",
          "filter": {
            "filterlist": [],
            "filtermergetype": "none",
            "filtermergeexpression": "",
            "keycolumns": ""
          }
        },
        {
          "sourcename": "param1",
          "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Coinsurance.CoinsuranceList]",
          "sourceelementtype": "repeater",
          "filter": {
            "filterlist": [],
            "filtermergetype": "none",
            "filtermergeexpression": "",
            "keycolumns": ""
          }
        }
      ],
      "sourcemergelist": {
        "outputcolumns": {
          "columns": "",
          "children": ""
        },
        "sourcemergeactions": [
          {
            "sourcemergetype": "script",
            "sourcemergeexpression": "SET(CoinsList, a);SET(SelectedCostShareRow, CREATETOKEN(param1));SET(costShareType, GETVAL(SelectedCostShareRow, \"CostShareType\"));SET(NetworkTier, GETVAL(SelectedCostShareRow, \"NetworkTier\"));SET(IsRefreshCoinsurance, GETVAL(SelectedCostShareRow, \"RefreshCoinsurance\"));SET(coinsAmount, GETVAL(SelectedCostShareRow, \"CoinsuranceAmount\"));SET(NetworkCondition, \"NetworkTier=[<0>]\");SET(NetworkCondition, REPLACE(NetworkCondition, \"<0>\", NetworkTier));SET(CoinsToken, FILTERLIST(CoinsList, NetworkCondition, \"\", \"\"));SET(CoinsValue, GETVAL(CoinsToken, \"CoinsuranceAmount\"));SET(nonCoinsCostShareType, \"Copay Only,Deductible Only,No Cost,Deductible then Copay,Copay then Deductible\");SET(PropToUpdate, \"{''CoinsuranceAmount'':''<0>''}\");SET(CoinsValue, \"\");IF(EQUALS(IsRefreshCoinsurance, \"Yes\")==TRUE()){SET(SameAsNetworkTier, GETVAL(SelectedCostShareRow, \"SameAsNetworkTier\")); SET(SameAsCostShare, GETVAL(SelectedCostShareRow, \"SameAsCostShare\")); SET(NetworkTier, \"\"); SET(SameAsNetworkTierApply, 0); IF(EQUALS(SameAsNetworkTier, \"\") < > TRUE()){IF(EQUALS(SameAsNetworkTier, \"Not Applicable\") < > TRUE()){IF(CONTAINS(SameAsCostShare, \"CoinsuranceAmount\")==TRUE()){SET(SameAsNetworkTierApply, 1);}}}IF(EQUALS(SameAsNetworkTierApply, 1)==TRUE()){SET(NetworkTier, GETVAL(SelectedCostShareRow, \"SameAsNetworkTier\"));}ELSE{SET(NetworkTier, GETVAL(SelectedCostShareRow, \"NetworkTier\"));}SET(tierCondition, \"NetworkTier=[<0>]\"); SET(tierCondition, REPLACE(tierCondition, \"<0>\", NetworkTier)); SET(CoinsToken, FILTERLIST(CoinsList, tierCondition, \"\", \"\", \"\")); SET(coinsAmount, GETVAL(CoinsToken, \"CoinsuranceAmount\")); IF(CONTAINS(nonCoinsCostShareType, costShareType)==TRUE()){SET(CoinsValue, \"Not Applicable\");}IF(EQUALS(costShareType, \"Not Covered\")==TRUE()){SET(CoinsValue, \"Not Covered\");}IF(EQUALS(CoinsValue, \"\") < > TRUE()){SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", CoinsValue));}ELSE{SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", coinsAmount));}}ELSE{SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", coinsAmount));}SET(SelectedCostShareRow, UPDATEARRAY(SelectedCostShareRow, NetworkCondition, PropToUpdate));SETARRAY(target, SelectedCostShareRow);",
            "keycolumns": "",
            "mappings": {
              "sourcefields": "",
              "targetfields": ""
            }
          }
        ]
      }
    }
  }
}'where SourcePath=N'CascadingCostShare.CostShareGroup.CostShareGroupList.RefreshCoinsurance'

update ML.ElementDocumentRule set RuleJSON=N'{
  "documentrule": {
    "targetelement": "CommercialMedicalAnchor[CascadingCostShare.CostShareGroup.CostShareGroupList]",
    "ruletype": "datasource",
    "targetelementtype": "field",
    "ruleconditions": {
      "sources": [
        {
          "sourcename": "A",
          "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Coinsurance.CoinsuranceList]",
          "sourceelementtype": "repeater",
          "filter": {
            "filterlist": [],
            "filtermergetype": "none",
            "filtermergeexpression": "",
            "keycolumns": ""
          }
        },
        {
          "sourcename": "param1",
          "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Coinsurance.CoinsuranceList]",
          "sourceelementtype": "repeater",
          "filter": {
            "filterlist": [],
            "filtermergetype": "none",
            "filtermergeexpression": "",
            "keycolumns": ""
          }
        }
      ],
      "sourcemergelist": {
        "outputcolumns": {
          "columns": "",
          "children": ""
        },
        "sourcemergeactions": [
          {
            "sourcemergetype": "script",
            "sourcemergeexpression": "SET(CoinsList, a);SET(SelectedCostShareRow, CREATETOKEN(param1));SET(costShareType, GETVAL(SelectedCostShareRow, \"CostShareType\"));SET(NetworkTier, GETVAL(SelectedCostShareRow, \"NetworkTier\"));SET(IsRefreshCoinsurance, GETVAL(SelectedCostShareRow, \"RefreshCoinsurance\"));SET(coinsAmount, GETVAL(SelectedCostShareRow, \"CoinsuranceAmount\"));SET(NetworkCondition, \"NetworkTier=[<0>]\");SET(NetworkCondition, REPLACE(NetworkCondition, \"<0>\", NetworkTier));SET(CoinsToken, FILTERLIST(CoinsList, NetworkCondition, \"\", \"\"));SET(CoinsValue, GETVAL(CoinsToken, \"CoinsuranceAmount\"));SET(nonCoinsCostShareType, \"Copay Only,Deductible Only,No Cost,Deductible then Copay,Copay then Deductible\");SET(PropToUpdate, \"{''CoinsuranceAmount'':''<0>''}\");SET(CoinsValue, \"\");IF(EQUALS(IsRefreshCoinsurance, \"Yes\")==TRUE()){SET(SameAsNetworkTier, GETVAL(SelectedCostShareRow, \"SameAsNetworkTier\")); SET(SameAsCostShare, GETVAL(SelectedCostShareRow, \"SameAsCostShare\")); SET(NetworkTier, \"\"); SET(SameAsNetworkTierApply, 0); IF(EQUALS(SameAsNetworkTier, \"\") < > TRUE()){IF(EQUALS(SameAsNetworkTier, \"Not Applicable\") < > TRUE()){IF(CONTAINS(SameAsCostShare, \"CoinsuranceAmount\")==TRUE()){SET(SameAsNetworkTierApply, 1);}}}IF(EQUALS(SameAsNetworkTierApply, 1)==TRUE()){SET(NetworkTier, GETVAL(SelectedCostShareRow, \"SameAsNetworkTier\"));}ELSE{SET(NetworkTier, GETVAL(SelectedCostShareRow, \"NetworkTier\"));}SET(tierCondition, \"NetworkTier=[<0>]\"); SET(tierCondition, REPLACE(tierCondition, \"<0>\", NetworkTier)); SET(CoinsToken, FILTERLIST(CoinsList, tierCondition, \"\", \"\", \"\")); SET(coinsAmount, GETVAL(CoinsToken, \"CoinsuranceAmount\")); IF(CONTAINS(nonCoinsCostShareType, costShareType)==TRUE()){SET(CoinsValue, \"Not Applicable\");}IF(EQUALS(costShareType, \"Not Covered\")==TRUE()){SET(CoinsValue, \"Not Covered\");}IF(EQUALS(CoinsValue, \"\") < > TRUE()){SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", CoinsValue));}ELSE{SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", coinsAmount));}}ELSE{SET(PropToUpdate, REPLACE(PropToUpdate, \"<0>\", coinsAmount));}SET(SelectedCostShareRow, UPDATEARRAY(SelectedCostShareRow, NetworkCondition, PropToUpdate));SETARRAY(target, SelectedCostShareRow);",
            "keycolumns": "",
            "mappings": {
              "sourcefields": "",
              "targetfields": ""
            }
          }
        ]
      }
    }
  }
}'where SourcePath=N'CascadingCostShare.CostShareGroup.CostShareGroupList.CostShareType'