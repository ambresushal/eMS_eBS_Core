INSERT [ML].[ElementDocumentRule] ([FormDesignID], [FormDesignVersionID], [SourceField], [SourcePath], [TargetFields], [TargetPaths], [RuleJSON], [AddedBy], [AddedDate], [RunOnMigration]) VALUES (2405, 2429, N'BenefitCategory3', N'ProductRules.SpecialServiceSelectionRepeater.BenefitCategory3', N'ProductRules.SpecialServiceSelectionRepeater', N'ProductRules.SpecialServiceSelectionRepeater', N'{
  "documentrule": {
    "targetelement": "CommercialMedicalAnchor[ProductRules.SpecialServiceSelectionRepeater]",
    "ruletype": "datasource",
    "targetelementtype": "field",
    "ruleconditions": {
      "sources": [
        {
          "sourcename": "A",
          "sourceelement": "StandardServices[SpecialServiceDetails.SpecialServiceGroupDetailRulesList]",
          "sourceelementtype": "repeater",
          "filter": {
            "filterlist": [],
            "filtermergetype": "none",
            "filtermergeexpression": "",
            "keycolumns": ""
          }
        },
        {
          "sourcename": "B",
          "sourceelement": "CommercialMedicalAnchor[ProductRules.PlanInformation.ServiceGroup]",
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
          "sourceelement": "CommercialMedicalAnchor[ProductRules.SpecialServiceSelectionRepeater]",
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
            "sourcemergeexpression": "SET(SelectedRow,CREATETOKEN(param1));SET(BenefitCategory1,GETVAL(SelectedRow,\"BenefitCategory1\"));SET(Condition,\"ServiceGroup=[<0>]&BenefitCategory1=[<1>]\");SET(Condition,REPLACE(Condition,\"<0>\",b));SET(Condition,REPLACE(Condition,\"<1>\",BenefitCategory1));SET(mlList,FILTERLIST(a,Condition,\"\",\"\"));SET(mlList,UNION(mlList,mlList,\"BenefitCategory3\"));SET(BlankBenefitCategory3List,FILTERLIST(mlList,\"BenefitCategory3=[]\",\"\",\"\"));IF(COUNT(BlankBenefitCategory3List)>0){SET(mlList,UPDATEARRAY(mlList,\"BenefitCategory3=[]\",\"{''BenefitCategory3'':''Blank''}\"));}SETARRAY(target,mlList);",
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
}', N'superuser', CAST(N'2019-03-13T00:00:00.000' AS DateTime), 0)