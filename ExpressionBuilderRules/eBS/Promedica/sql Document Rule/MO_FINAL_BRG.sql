update ML.ElementDocumentRule set RuleJSON=N'{
    "documentrule": {
        "targetelement": "CommercialMedicalAnchor[BenefitReview.BenefitReviewGrid]",
        "ruletype": "datasource",
        "targetelementtype": "field",
        "ruleconditions": {
            "sources": [{
                "sourcename": "A",
                "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Coinsurance.CoinsuranceList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "B",
                "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.Deductible.DeductibleList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "C",
                "sourceelement": "CommercialMedicalAnchor[CascadingCostShare.CostShareGroup.CostShareGroupList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "D",
                "sourceelement": "CommercialMedicalAnchor[AdditionalServices.AdditionalServiceList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "E",
                "sourceelement": "CommercialMedicalAnchor[StandardServices.StandardServiceList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "F",
                "sourceelement": "CommercialMedicalAnchor[Network.NetworkTierList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "G",
                "sourceelement": "CommercialMedicalAnchor[BenefitReview.BenefitReviewGrid]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "H",
                "sourceelement": "CommercialMedicalAnchor[GeneralCostShare.OutofPocketMaximum.OutofPocketMaximumList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "J",
                "sourceelement": "CommercialMedicalAnchor[Network.CoverageLevelList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "K",
                "sourceelement": "CommercialMedicalAnchor[ProductRules.PlanInformation.ServiceGroup]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "L",
                "sourceelement": "StandardServices[StandardServiceGroupDetailRules.StandardServiceGroupDetailRuleList]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }, {
                "sourcename": "param1",
                "sourceelement": "CommercialMedicalAnchor[ProductRules.PlanInformation.ServiceGroup]",
                "sourceelementtype": "repeater",
                "filter": {
                    "filterlist": [],
                    "filtermergetype": "none",
                    "filtermergeexpression": "",
                    "keycolumns": ""
                }
            }],
            "sourcemergelist": {
                "outputcolumns": {
                    "columns": "",
                    "children": ""
                },
                "sourcemergeactions": [{
                    "sourcemergetype": "script",
                    "sourcemergeexpression": "SET(inputRow, param1);SET(inputToken, CREATETOKEN(inputRow));SET(networkTierValue, GETVAL(inputToken, \"NetworkTier\"));SET(networkNameValue, GETVAL(inputToken, \"NetworkName\"));SET(rowID, GETVAL(inputToken, \"RowIDProperty\"));SET(limits, GETVAL(inputToken, \"Limits\"));SET(benefitServiceCode, GETVAL(inputToken, \"BenefitServiceCode\"));SET(costShareGroupList, c);SET(coinsuranceCostShare, a);SET(oopmCostShare, h);SET(deductibleCostShare, b);SET(tierCondition, \"NetworkTier=[<0>]\");SET(tierCondition, REPLACE(tierCondition, \"<0>\", networkTierValue));SET(costShareCondition, \"NetworkTier=[<0>]&CostShareGroup=[<1>]\");SET(sgFilterCondition, \"BenefitServiceCode=[serviceValue]\");SET(sgFilterCondition, REPLACE(sgFilterCondition, \"serviceValue\", benefitServiceCode));SET(resultServiceRow, FILTERLIST(e, sgFilterCondition, \"\", \"\", \"\"));SET(resultServiceRowFromML, FILTERLIST(l, sgFilterCondition, \"\", \"\", \"\"));SET(resultServiceRow, MERGEARRAY(resultServiceRow, resultServiceRowFromML, \"BenefitServiceCode\"));SET(costShareGroupValue, FILTERLIST(resultServiceRow, sgFilterCondition, \"\", \"\", \"CostShareGroup\"));SET(costShareCondition, REPLACE(costShareCondition, \"<0>\", networkTierValue));SET(costShareCondition, REPLACE(costShareCondition, \"<1>\", costShareGroupValue));SET(defaultPropToMerge, \"{''NetworkTier'':''gp'',''NetworkName'':''Value1'',''Covered'':''Yes'',''Copay'':'''',''ManualOverride'':''No'',''Coinsurance'':'''',''IndDeductible'':'''',''FamDeductible'':'''',''IndOOPM'':'''',''TwoPersonOOPM'':'''',''FamilyOOPM'':'''',''RowIDProperty'':'''',''Limits'':''No'',''SameAsNetworkTier'':'''',''SameAsCostShare'':'''',''CopayFrequency'':'''',''TwoPersonDeductible'':'''',''CopayType'':'''',''CostShareType'':'''',''CopaySupplement'':''''}\");SET(defaultPropToMerge, REPLACE(defaultPropToMerge, \"gp\", networkTierValue));SET(defaultPropToMerge, REPLACE(defaultPropToMerge, \"Value1\", networkNameValue));SET(propertyToMerge, CREATETOKEN(defaultPropToMerge));SET(resultServiceRow, CROSSJOIN(resultServiceRow, propertyToMerge));SET(costSharegroupData, FILTERLIST(costShareGroupList, costShareCondition, \"\", \"\", \"\"));SET(costShareType, GETVAL(costSharegroupData, \"CostShareType\"));SET(CopaySupplementValue, GETVAL(costSharegroupData, \"CopaySupplement\"));SET(updateToken, \"{''Copay'':''<0>'',''Coinsurance'':''<1>'',''IndDeductible'':''<2>'',''FamDeductible'':''<3>'',''IndOOPM'':''<4>'',''FamilyOOPM'':''<5>'',''ReductionofBenefits'':''<6>'',''RowIDProperty'':''<7>'',''Limits'':''<8>'',''SameAsNetworkTier'':''<9>'',''SameAsCostShare'':''<10>'',''CopayFrequency'':''<11>'',''CopayType'':''<12>'',''ManualOverride'':''<13>'',''TwoPersonDeductible'':''<14>'',''TwoPersonOOPM'':''<15>'',''CostShareType'':''<16>'',''CopaySupplement'':''<17>''}\");SET(coinsRow, FILTERLIST(coinsuranceCostShare, tierCondition, \"\", \"\", \"\"));SET(cvgIndCondition, tierCondition);SET(cvgFamCondition, tierCondition);SET(cvgTwoPerCondition, tierCondition);SET(IndTierCondition, APPEND(cvgIndCondition, \"&CoverageName=[Individual]\"));SET(FamTierCondition, APPEND(cvgFamCondition, \"&CoverageName=[Family]\"));SET(TwoPerTierCondition, APPEND(cvgTwoPerCondition, \"&CoverageName=[Two Person]\"));SET(IndDeductibleRow, FILTERLIST(deductibleCostShare, IndTierCondition, \"\", \"\", \"\"));SET(FamDeductibleRow, FILTERLIST(deductibleCostShare, FamTierCondition, \"\", \"\", \"\"));SET(TwoPerDeductibleRow, FILTERLIST(deductibleCostShare, TwoPerTierCondition, \"\", \"\", \"\"));SET(IndOOPMRow, FILTERLIST(oopmCostShare, IndTierCondition, \"\", \"\", \"\"));SET(FamOOPMRow, FILTERLIST(oopmCostShare, FamTierCondition, \"\", \"\", \"\"));SET(TwoPerOOPMRow, FILTERLIST(oopmCostShare, TwoPerTierCondition, \"\", \"\", \"\"));SET(CopayValue, GETVAL(costSharegroupData, \"CopayValue\"));SET(IndOOPMValue, GETVAL(IndOOPMRow, \"OOPMAmount\"));SET(FamOOPMValue, GETVAL(FamOOPMRow, \"OOPMAmount\"));SET(TwoPerOOPMValue, GETVAL(TwoPerOOPMRow, \"OOPMAmount\"));SET(IndDeductibleValue, \"\");SET(FamDeductibleValue, \"\");SET(TwoPerDeductibleValue, \"\");SET(SameAsNetworkTier, GETVAL(costSharegroupData, \"SameAsNetworkTier\"));SET(SameAsCostShare, GETVAL(costSharegroupData, \"SameAsCostShare\"));SET(IndDeductibleValue, GETVAL(IndDeductibleRow, \"DeductibleAmount\"));SET(FamDeductibleValue, GETVAL(FamDeductibleRow, \"DeductibleAmount\"));SET(TwoPerDeductibleValue, GETVAL(TwoPerDeductibleRow, \"DeductibleAmount\"));IF(EQUALS(SameAsNetworkTier, \"\") < > TRUE()){IF(CONTAINS(SameAsCostShare, \"Deductible\")==TRUE()){SET(SameAstierCondition, \"NetworkTier=[<0>]\"); SET(SameAstierCondition, REPLACE(SameAstierCondition, \"<0>\", SameAsNetworkTier)); SET(SameAscvgIndCondition, SameAstierCondition); SET(SameAscvgFamCondition, SameAstierCondition); SET(SameAscvgTwoPerCondition, SameAstierCondition); SET(SameAscvgIndCondition, APPEND(SameAscvgIndCondition, \"&CoverageName=[Individual]\")); SET(SameAscvgFamCondition, APPEND(SameAscvgFamCondition, \"&CoverageName=[Family]\")); SET(SameAscvgTwoPerCondition, APPEND(SameAscvgTwoPerCondition, \"&CoverageName=[Two Person]\")); SET(IndDeductibleRow, FILTERLIST(deductibleCostShare, SameAscvgIndCondition, \"\", \"\", \"\")); SET(FamDeductibleRow, FILTERLIST(deductibleCostShare, SameAscvgFamCondition, \"\", \"\", \"\")); SET(TwoPerDeductibleRow, FILTERLIST(deductibleCostShare, SameAscvgTwoPerCondition, \"\", \"\", \"\"));}}SET(IndDeductibleValue, GETVAL(IndDeductibleRow, \"DeductibleAmount\"));SET(FamDeductibleValue, GETVAL(FamDeductibleRow, \"DeductibleAmount\"));SET(TwoPerDeductibleValue, GETVAL(TwoPerDeductibleRow, \"DeductibleAmount\"));SET(coinsurenceValue, GETVAL(coinsRow, \"CoinsuranceAmount\"));SET(updateToken, REPLACE(updateToken, \"<0>\", CopayValue));SET(updateToken, REPLACE(updateToken, \"<1>\", coinsurenceValue));SET(updateToken, REPLACE(updateToken, \"<2>\", IndDeductibleValue));SET(updateToken, REPLACE(updateToken, \"<3>\", FamDeductibleValue));SET(updateToken, REPLACE(updateToken, \"<4>\", IndOOPMValue));SET(updateToken, REPLACE(updateToken, \"<5>\", FamOOPMValue));SET(updateToken, REPLACE(updateToken, \"<7>\", rowID));SET(updateToken, REPLACE(updateToken, \"<8>\", limits));SET(updateToken, REPLACE(updateToken, \"<9>\", SameAsNetworkTier));SET(updateToken, REPLACE(updateToken, \"<10>\", SameAsCostShare));SET(updateToken, REPLACE(updateToken, \"<11>\", GETVAL(costSharegroupData, \"CopayFrequency\")));SET(updateToken, REPLACE(updateToken, \"<12>\", GETVAL(costSharegroupData, \"CopayType\")));SET(updateToken, REPLACE(updateToken, \"<13>\", GETVAL(inputToken, \"ManualOverride\")));SET(updateToken, REPLACE(updateToken, \"<14>\", TwoPerDeductibleValue));SET(updateToken, REPLACE(updateToken, \"<15>\", TwoPerOOPMValue));SET(updateToken, REPLACE(updateToken, \"<16>\", costShareType));SET(updateToken, REPLACE(updateToken, \"<17>\", CopaySupplementValue));SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, updateToken));IF(EQUALS(costShareType, \"Copay Only\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Coinsurance'':''Not Applicable'',''IndDeductible'':''Not Applicable'',''FamDeductible'':''Not Applicable'',''TwoPersonDeductible'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Coinsurance Only\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Copay'':''Not Applicable'',''IndDeductible'':''Not Applicable'',''FamDeductible'':''Not Applicable'',''TwoPersonDeductible'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Deductible Only\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Copay'':''Not Applicable'',''Coinsurance'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Deductible then Coinsurance\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Copay'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Deductible then Copay\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Coinsurance'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Copay then Deductible\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Coinsurance'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Copay then Coinsurance\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''IndDeductible'':''Not Applicable'',''FamDeductible'':''Not Applicable'',''TwoPersonDeductible'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal))}IF(EQUALS(costShareType, \"No Cost\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Copay'':''$0.00'',''Coinsurance'':''Not Applicable'',''IndDeductible'':''Not Applicable'',''FamDeductible'':''Not Applicable'',''TwoPersonDeductible'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"Not Covered\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''Copay'':''Not Covered'',''Coinsurance'':''Not Covered'',''IndDeductible'':''Not Covered'',''FamDeductible'':''Not Covered'',''TwoPersonDeductible'':''Not Covered'',''Covered'':''No''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal));}IF(EQUALS(costShareType, \"\")==TRUE()){SET(costSharegroupListTokenFinal, \"{''IndDeductible'':''Not Applicable'',''FamDeductible'':''Not Applicable'',''TwoPersonDeductible'':''Not Applicable''}\"); SET(resultServiceRow, UPDATEARRAY(resultServiceRow, tierCondition, costSharegroupListTokenFinal))}SETARRAY(target, resultServiceRow);",
                    "keycolumns": "",
                    "mappings": {
                        "sourcefields": "",
                        "targetfields": ""
                    }
                }]
            }
        }
    }
}'where SourcePath='BenefitReview.BenefitReviewGrid.ManualOverride'