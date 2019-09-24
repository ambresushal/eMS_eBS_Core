var folderVersionWorkflowInstance = function () {

    //variable to hold the instance
    var instance;

    var newVersionLoaded = true;
    function folderVersionWorkflow(accountId, folderVersionId, folderId) {
        this.accountId = accountId;
        this.folderVersionId = folderVersionId;
        this.folderId = folderId;

        this.windowWidth = $(window).width();
        this.checkWidth = 0;
        this.windowSize = 0;
        this.windowNewSize = 0;
        this.totalWizardListCount = 0;
        this.listItems = 0;
        this.listItemsWidth = 0;
        this.firstVisibleIndex = 0;
        this.lastVisibleIndex = 0;
        this.widthChanged = false;
        this.currentFolderStateIndex = 0;
        this.workFlowStates = null;
        this.workFlowList = null;
        var currentInstance = this;
        instance = currentInstance;

        //urls to be accessed for folder version
        var URLs = {
            //get Folder Version Workflow
            folderVersionWorkflowList: '/FolderVersionWorkFlow/GetFolderVersionWorkFlowList?tenantId=1&folderVersionId={folderVersionId}',
            workFlowStateList: '/FolderVersionWorkFlow/GetWorkFlowStateList?tenantId=1&folderVersionId={folderVersionId}'
        };

        //element ID's required for form design
        //added in Views/FolderVersion/Index.cshtml
        var elementIDs = {
            //container element for Folder Version Workflow List
            folderVersionWorkflowContainer: '#workflowdivJQ',
            //Next Button element
            buttonNext: '#nextJQ',
            //Last Button element
            buttonLast: '#lastJQ',
            //First Button element
            buttonFirst: '#firstJQ',
            //Previous Button element
            buttonPrevious: '#previousJQ',
            //Dropdown Button element
            buttonDropDown: '#downJQ',
            //Dropdown workflow list
            dropDownWorkflowList: '#dropdownworkflowlistJQ',
            //Container showing version number adjacent to Workflow
            workflowContainer: '#workflowContainerJQ',
            folderVersionAlertJQ: '#folderVersionAlert',
            retroAlertJQ: '.retroAlert'
        };

        return {
            load: function () {
                //TODO : add code to workflow for the given form here.
                //TODO: assign the loaded object to "data" to data to return 
                var urlGetWFStates = URLs.workFlowStateList.replace(/\{folderVersionId\}/g, folderData.folderVersionId);
                var promise = ajaxWrapper.getJSON(urlGetWFStates);
                promise.done(function (data) {
                    var allWorkflowList = data;
                    var url = URLs.folderVersionWorkflowList.replace(/\{folderVersionId\}/g, folderVersionId);
                    var promise = ajaxWrapper.getJSON(url);
                    //register ajax success callback
                    promise.done(function (xhr) {
                        currentInstance.workFlowStates = xhr;
                        currentInstance.workFlowList = allWorkflowList;
                        loadWorkflowStateList(xhr, allWorkflowList);
                    });
                    promise.fail(showError);
                });
                //register ajax failure callback
                promise.fail(showError);
            },
            getWorkFlowStates: function () {
                return currentInstance.workFlowStates;
            },
            getWorkFlowList: function () {
                return currentInstance.workFlowList;
            }
        }


        // finction to display workflow  list with approved, not approved and not applicable indication
        function loadWorkflowStateList(workFlowState, workFlowList) {
            //load all workflow states with active one
            var approvedStatesCount = 0;
            $(elementIDs.folderVersionWorkflowContainer + ' li').remove();
            if (folderData.versionType == VersionType.Retro && (folderData.folderVersionState == FolderVersionState.INPROGRESS_BLOCKED || folderData.folderVersionState == FolderVersionState.INPROGRESS)) {
                $(elementIDs.retroAlertJQ).css("display", "block");
            }

            if (folderData.isNewLoadedVersionIsMajorVersion == "True") {
                $(elementIDs.folderVersionAlertJQ).show();
                $(elementIDs.folderVersionAlertJQ).fadeOut(7000);
                folderData.isNewLoadedVersionIsMajorVersion = false;
            }
            var preWFSequence = 0;
            //List of all available workflow states
            $.each(workFlowList, function (i, item) {

                var WorkFlowClassName = item.WFStateName.length <= WorkFlowDiv.AverageTextLength ? WorkFlowDiv.MinWidthClass : WorkFlowDiv.MaxWidthClass;

                $(elementIDs.folderVersionWorkflowContainer).append('<li id="ul-id-' + (i + 1) + '" class=' + WorkFlowClassName + '><span class="workflowspan" style="position: absolute; margin: -1px 9.9px; ">' + item.WFStateName + '</span></li>');
                $.each(workFlowState, function (j, workflow) {
                    if ((item.WorkFlowVersionStateID === workflow.WorkflowStateID && workflow.ApprovalStatusID === ApprovalStatus.NOTAPPROVED)) {
                        $(elementIDs.folderVersionWorkflowContainer).find('#ul-id-' + (i + 1) + '').addClass('active');
                        currentInstance.currentFolderStateIndex = i;
                    }
                    if ((item.WorkFlowVersionStateID == workflow.WorkflowStateID) && ((workflow.ApprovalStatusID === ApprovalStatus.COMPLETED) || (workflow.ApprovalStatusID === ApprovalStatus.APPROVED) || (item.WFStateName == WorkFlowState.MarketApproval && workflow.ApprovalStatusID === ApprovalStatus.APPROVED))) {
                        $('#ul-id-' + (i + 1)).addClass('approved');
                        $('#firstJQ').addClass('approved');
                        // $('#ul-id-' + (i + 1)).append('<span class="glyphicon glyphicon-check workflow-graphic-icon "></span>');
                    }

                    if (item.WorkFlowVersionStateID == workflow.WorkflowStateID && workflow.ApprovalStatusID === ApprovalStatus.NOTAPPLICABLE) {
                        $('#ul-id-' + (i + 1)).addClass('approved notapplicable');
                        //$('#ul-id-' + (i + 1)).append('<span class="glyphicon glyphicon-unchecked workflow-graphic-icon text-black"></span>');
                    }
                    if (item.WorkFlowVersionStateID == workflow.WorkflowStateID && workflow.ApprovalStatusID === ApprovalStatus.ACCELERATED) {
                        $('#ul-id-' + (i + 1)).addClass('approved accelerated');
                        //$('#ul-id-' + (i + 1)).append('<span class="glyphicon glyphicon-forward workflow-graphic-icon text-green"></span>');
                    }
                    if (item.WorkFlowVersionStateID == workflow.WorkflowStateID && workflow.ApprovalStatusID === ApprovalStatus.ERROR) {
                        $('#ul-id-' + (i + 1)).addClass('approved error');
                        //$('#ul-id-' + (i + 1)).append('<span class="glyphicon glyphicon-exclamation-sign workflow-graphic-icon text-red"></span>');
                    }
                    
                });
                if($(elementIDs.folderVersionWorkflowContainer).find('li').hasClass('approved')){
                    approvedStatesCount++;
                }
                if(approvedStatesCount === workFlowList.length){
                    $('#lastJQ').addClass('approved');
                    $(elementIDs.folderVersionWorkflowContainer).addClass('full-approved');
                }
            });

            $(elementIDs.folderVersionWorkflowContainer).animate({

                scrollLeft: "+=" + currentInstance.currentFolderStateIndex * 190 + "px"
            });

            currentInstance.windowWidth = $(window).width();
            currentInstance.checkWidth = 0;
            currentInstance.windowSize = 0;
            currentInstance.windowNewSize = 0;
            currentInstance.totalWizardListCount = $('' + elementIDs.folderVersionWorkflowContainer + ' li').length;
            currentInstance.listItems = $('' + elementIDs.folderVersionWorkflowContainer + ' li');
            currentInstance.listItemsWidth = 0;
            currentInstance.firstVisibleIndex = 0;
            currentInstance.lastVisibleIndex = 0;
            currentInstance.widthChanged = false;

            //Window resize event 
            // $(window).resize(function () {
            //     currentInstance.windowWidth = $(window).width();
            //     currentInstance.checkWidth = (WorkFlowDiv.MAXWIDTH * currentInstance.windowWidth) / 100;
            //     currentInstance.listItemsWidth = 0;
            //     var count = 0;
            //     currentInstance.listItems.each(function (idx, li) {
            //         var product = $(li);

            //         currentInstance.listItemsWidth = currentInstance.listItemsWidth + product.width();

            //         if (currentInstance.listItemsWidth < currentInstance.checkWidth) {
            //             product.show();

            //         } else {
            //             product.hide();
            //             currentInstance.lastVisibleIndex = idx;
            //             count = count + 1;
            //         }


            //         ////$(elementIDs.buttonFirst).attr('disabled', 'disabled');
            //         //$(elementIDs.buttonPrevious).attr('disabled', 'disabled');
            //         if (count > 0) {
            //             //$(elementIDs.buttonLast).removeAttr('disabled');
            //             //$(elementIDs.buttonNext).removeAttr('disabled');
            //         } else {
            //             ////$(elementIDs.buttonNext).attr('disabled', 'disabled');
            //             //$(elementIDs.buttonNext).attr('disabled', 'disabled');
            //         }
            //         currentInstance.firstVisibleIndex = 0;
            //     });
            //     currentInstance.listItems.each(function (idx, li) {
            //         var product = $(li);
            //         if (product.is(':visible')) {
            //             currentInstance.lastVisibleIndex = idx;
            //             return true;
            //         }
            //     });
            //     displayCurrentWorkFlowStateOfFolder();
            // });


            /*After loading if workflow state list width is greater then container width 
            then display next and last button and hide first and previous buttons */
            currentInstance.listItems.each(function (idx, li) {
                currentInstance.checkWidth = (WorkFlowDiv.MAXWIDTH * currentInstance.windowWidth) / 100;
                var product = $(li);
                var count = 0;
                currentInstance.listItemsWidth = currentInstance.listItemsWidth + product.width();
                if (currentInstance.listItemsWidth <= currentInstance.checkWidth) {

                   // product.show();
                }
                else {
                    count = count + 1;
                    //product.hide();
                }
                //$(elementIDs.buttonFirst).attr('disabled', 'disabled');
                $(elementIDs.buttonPrevious).attr('disabled', 'disabled');
                if (count > 0) {
                    //$(elementIDs.buttonLast).removeAttr('disabled');
                    //$(elementIDs.buttonNext).removeAttr('disabled');
                } else {
                    ////$(elementIDs.buttonNext).attr('disabled', 'disabled');
                    //$(elementIDs.buttonNext).attr('disabled', 'disabled');
                }
            });

            // Apply tooltip for all workflow states 
            currentInstance.listItems.each(function (idx, li) {
                var text = $(li).text();
                var control = $(li);
                $(li).find('.workflowspan').attr('title', text).tooltip({ container: 'body' });

                //var tooltipControlText = '.workflowspan';
                //addTooltip(control, tooltipControlText);

                $(li).find('.glyphicon-check').attr('title', 'Approved');
                $(li).find('.glyphicon-check').attr('data-placement', 'bottom');

                var tooltipControlApproved = '.glyphicon-check';
                addTooltip(control, tooltipControlApproved);

                $(li).find('.glyphicon-unchecked').attr('title', 'Not Applicable');
                $(li).find('.glyphicon-unchecked').attr('data-placement', 'bottom');

                var tooltipControlNotApplicable = '.glyphicon-unchecked';
                addTooltip(control, tooltipControlNotApplicable);

                $(li).find('.glyphicon-forward').attr('title', 'Accelerated');
                $(li).find('.glyphicon-forward').attr('data-placement', 'bottom');

                var tooltipControlAccelerated = '.glyphicon-forward';
                addTooltip(control, tooltipControlAccelerated);


                // display specified lengths of characters 
                //if ($(li).text().length > 15) {
                //    $(li).find('.workflowspan').text(text.substring(0, 15) + "...");
                //}

                //proper alignment of image
                if (idx == 0) {
                    $(li).find('.workflow-graphic-icon').css('left', '0px');
                } else if (idx == currentInstance.totalWizardListCount - 1) {
                    $(li).find('.workflow-graphic-icon').css('left', '13px');
                } else {
                    $(li).find('.workflow-graphic-icon').css('left', '-3px');
                    $(li).find('.workflow-graphic-icon').css('margin-left', '24px');
                }

            });

            function addTooltip(control, tooltipControl) {
                $(control).find('' + tooltipControl).mouseover(function () {
                    $(control).find('' + tooltipControl).tooltip('show');
                    $('.tooltip-inner').css('background-color', '#3693D0');
                    $('.tooltip.top .tooltip-arrow').css('border-top-color', '#3693D0');
                    $('.tooltip.right .tooltip-arrow').css('border-right-color', '#3693D0');
                    $('.tooltip.left .tooltip-arrow').css('border-left-color', '#3693D0');
                    $('.tooltip.bottom .tooltip-arrow').css('border-bottom-color', '#3693D0');
                });
            }

            //return index of visible workflow states
            currentInstance.listItems.each(function (idx, li) {
                var product = $(li);
                if (product.is(':visible')) {
                    currentInstance.lastVisibleIndex = idx;
                    return true;
                }
            });

            displayCurrentWorkFlowStateOfFolder();

            // code for next button click event
            $(elementIDs.buttonNext).click(function () {
                if (currentInstance.lastVisibleIndex < currentInstance.totalWizardListCount) {
                    showWorkFlowStates(currentInstance.lastVisibleIndex, currentInstance.firstVisibleIndex, 1);
                    disableEnableScrollingButtons();
                    return true;
                } else {
                    return false;
                }
            });

            // code for last button click event
            $(elementIDs.buttonLast).click(function () {
                event.preventDefault();
                $(elementIDs.folderVersionWorkflowContainer).animate({
                    scrollLeft: "+=200px"
                  }, "slow");
                // currentInstance.checkWidth = (WorkFlowDiv.MAXWIDTH * currentInstance.windowWidth) / 100;
                // currentInstance.listItemsWidth = 0;
                // $(currentInstance.listItems.get().reverse()).each(function (index) {
                //     currentInstance.listItemsWidth = currentInstance.listItemsWidth + $(this).width();
                //     //code to show same number of items
                //     if (index > 2 && currentInstance.listItemsWidth < currentInstance.checkWidth)
                //         currentInstance.listItemsWidth = currentInstance.checkWidth;

                //     if (currentInstance.listItemsWidth < currentInstance.checkWidth) {
                //         $(this).show();
                //         currentInstance.firstVisibleIndex = $(this).index();
                //     } else {
                //         $(this).hide();
                //     }

                // });
                // currentInstance.lastVisibleIndex = currentInstance.totalWizardListCount - 1;
                // disableEnableScrollingButtons();
            });

            // code for previous button click event
            $(elementIDs.buttonPrevious).click(function () {
                if (currentInstance.firstVisibleIndex > 0) {
                    showWorkFlowStates(currentInstance.firstVisibleIndex, currentInstance.lastVisibleIndex, -1);
                    disableEnableScrollingButtons();
                    return true;
                } else {
                    return false;
                }

            });

            // code for first button click event
            $(elementIDs.buttonFirst).click(function () {
                event.preventDefault();
                $(elementIDs.folderVersionWorkflowContainer).animate({
                    scrollLeft: "-=200px"
                  }, "slow");
                // currentInstance.checkWidth = (WorkFlowDiv.MAXWIDTH * currentInstance.windowWidth) / 100;
                // currentInstance.listItemsWidth = 0;
                // $(currentInstance.listItems).each(function (idx, li) {
                //     var product = $(li);
                //     currentInstance.listItemsWidth = currentInstance.listItemsWidth + product.width();
                //     if (currentInstance.listItemsWidth > currentInstance.checkWidth) {
                //         product.hide();

                //     } else {
                //         product.show();
                //         currentInstance.lastVisibleIndex = $(this).index();
                //     }
                // });
                // currentInstance.firstVisibleIndex = 0;
                // disableEnableScrollingButtons();
            });

            //code for append workflow list on dorpdown button
            $(elementIDs.buttonDropDown).click(function () {
                $(elementIDs.dropDownWorkflowList).remove();
                $(elementIDs.buttonDropDown).parent().append('<ul id="dropdownworkflowlistJQ" class="workflow-ul dropdown-menu pull-right">');
                $.each(workFlowList, function (i, item) {
                    $(elementIDs.dropDownWorkflowList).append('<li id="li-id-' + (i + 1) + '" class="workflow-li "><span class="spanwidth">' + item.WFStateName + '</span></li>');
                    $.each(workFlowState, function (j, workflow) {
                        if (item.WorkFlowVersionStateID === workflow.WorkflowStateID && workflow.ApprovalStatusID === ApprovalStatus.NOTAPPROVED) {
                            $(elementIDs.dropDownWorkflowList).find('#li-id-' + (i + 1) + '').addClass('active');
                        }
                    });
                });
                $(elementIDs.buttonDropDown).append('</ul>');
            });

        }

        function displayCurrentWorkFlowStateOfFolder() {
            while (currentInstance.lastVisibleIndex < currentInstance.currentFolderStateIndex) {
                showWorkFlowStates(currentInstance.lastVisibleIndex, currentInstance.firstVisibleIndex, 1);
            }
            disableEnableScrollingButtons();
        }

        function disableEnableScrollingButtons() {
            if (currentInstance.lastVisibleIndex === currentInstance.totalWizardListCount - 1) {
                ////$(elementIDs.buttonNext).attr('disabled', 'disabled');
                //$(elementIDs.buttonNext).attr('disabled', 'disabled');
                rightScrollEnable = false;
            } else {
                //$(elementIDs.buttonLast).removeAttr('disabled');
                //$(elementIDs.buttonNext).removeAttr('disabled');
            }
            if (currentInstance.firstVisibleIndex == 0) {
                //$(elementIDs.buttonFirst).attr('disabled', 'disabled');
                $(elementIDs.buttonPrevious).attr('disabled', 'disabled');
                leftScrollEnable = false;
            } else {
                //$(elementIDs.buttonFirst).removeAttr('disabled');
                $(elementIDs.buttonPrevious).removeAttr('disabled');
            }
        }

        function showWorkFlowStates(showIndex, hideIndex, offSet) {
            currentInstance.listItemsWidth = 0;
            currentInstance.checkWidth = (WorkFlowDiv.MAXWIDTH * currentInstance.windowWidth) / 100;
            var showindex = showIndex;
            var hideindex = hideIndex;
            for (var i = showindex; i <= hideindex; i++) {
                currentInstance.listItemsWidth = currentInstance.listItemsWidth + $('' + elementIDs.folderVersionWorkflowContainer + ' li').eq(i).width();
            }
            $('' + elementIDs.folderVersionWorkflowContainer + ' li').eq(hideindex).hide();
            if (currentInstance.listItemsWidth > currentInstance.checkWidth) {
                var nexthideindex = hideindex + offSet;
                $('' + elementIDs.folderVersionWorkflowContainer + ' li').eq(nexthideindex).hide();
            }
            $('' + elementIDs.folderVersionWorkflowContainer + ' li').eq(showindex + offSet).show();
            currentInstance.lastVisibleIndex = currentInstance.lastVisibleIndex + offSet;
            currentInstance.firstVisibleIndex = currentInstance.firstVisibleIndex + offSet;
        }
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //variable to hold the instance
    var instance;
    return {
        getInstance: function (accountId, folderVersionId, folderId) {
            if (instance === undefined) {
                instance = new folderVersionWorkflow(accountId, folderVersionId, folderId);
            }
            else if (instance.accountId == accountId && instance.folderVersionId == folderVersionId && instance.folderId == folderId) {
                return instance
            }
            else {
                instance = undefined;
                instance = new folderVersionWorkflow(accountId, folderVersionId, folderId);
            }
            return instance;
        }

    }
}();


