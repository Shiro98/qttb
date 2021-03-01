app.controller("FlexValueController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.FLEX_VALUE_SET_ID = 0;
    
    $scope.modelSearch.SortColumn = "FLEX_VALUE_ID DESC";
    $scope.ListData = [];
    $scope.ListFlexValueSet = [];
    $scope.ListColumn = [];

    angular.element(document).ready(function () {
        showToast();
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                    });
                }
                $scope.ListFlexValueSet = response.FlexValueSets;
                $scope.modelSearch.FLEX_VALUE_SET_ID = $scope.ListFlexValueSet[0].FLEX_VALUE_SET_ID;
                $scope.ListColumn = response.FlexValueColumns;
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListData = data.data;
                $scope.$apply();
                 hideLoading();
            }
        });
    };

    $scope.Refesh = function () {
        $scope.LoadPage();
    };

    $scope.Sort = function (event, sortRow) {
        if (event.currentTarget.classList.contains('arrow-up')) {
            $scope.modelSearch.SortColumn = sortRow + " DESC";
            $scope.LoadPage();
        }
        if (event.currentTarget.classList.contains('arrow-down')) {
            $scope.modelSearch.SortColumn = sortRow + " ASC";
            $scope.LoadPage();
        }
    };

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/FlexValue/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.edit = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/FlexValue/_Edit',
            controller: 'edit',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                itemId: function () {
                    return itemId;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.delete = function (itemId, name) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa ' + name + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/FlexValue/Delete',
                            data: { Id: itemId  },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    //$scope.cancel();
                                    $scope.LoadPage();
                                }
                            }
                        });

                    }
                },
                close: {
                    text: 'Hủy',
                    action: function (scope, button) {

                    }
                }
            }
        });
    };
    $scope.GetValue = function (columnName, item) {
        if (columnName == 'FLEX_VALUE_ID')
            return item.FLEX_VALUE_ID;

        if (columnName == 'FLEX_VALUE_SET_ID')
            return item.FLEX_VALUE_SET_ID;

        if (columnName == 'FLEX_VALUE')
            return item.FLEX_VALUE;

        if (columnName == 'LAST_UPDATE_DATE')
            return item.LAST_UPDATE_DATE;

        if (columnName == 'LAST_UPDATED_BY')
            return item.LAST_UPDATED_BY;

        if (columnName == 'CREATION_DATE')
            return item.CREATION_DATE;

        if (columnName == 'CREATED_BY')
            return item.CREATED_BY;

        if (columnName == 'DESCRIPTION')
            return item.DESCRIPTION;

        if (columnName == 'ENABLED_FLAG')
            return item.ENABLED_FLAG;

        if (columnName == 'SUMMARY_FLAG')
            return item.SUMMARY_FLAG;

        if (columnName == 'PARENT_FLEX_VALUE_ID')
            return item.PARENT_FLEX_VALUE_ID;

        if (columnName == 'HIERARCHY_LEVEL')
            return item.HIERARCHY_LEVEL;

        if (columnName == 'FLEX_VALUE_CATEGORY')
            return item.FLEX_VALUE_CATEGORY;

        if (columnName == 'ATTRIBUTE1')
            return item.ATTRIBUTE1;

        if (columnName == 'ATTRIBUTE2')
            return item.ATTRIBUTE2;

        if (columnName == 'ATTRIBUTE3')
            return item.ATTRIBUTE3;

        if (columnName == 'ATTRIBUTE4')
            return item.ATTRIBUTE4;

        if (columnName == 'ATTRIBUTE5')
            return item.ATTRIBUTE5;

        if (columnName == 'ATTRIBUTE6')
            return item.ATTRIBUTE6;

        if (columnName == 'ATTRIBUTE7')
            return item.ATTRIBUTE7;

        if (columnName == 'ATTRIBUTE8')
            return item.ATTRIBUTE8;

        if (columnName == 'ATTRIBUTE9')
            return item.ATTRIBUTE9;

        if (columnName == 'ATTRIBUTE10')
            return item.ATTRIBUTE10;
    };
});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ListColumn = [];
    $scope.ListFlexValueSet = [];
    $scope.ListFlexValueParent = [];
    $scope.ListFlexValueCate = [];
    
    $scope.model = {};
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });
   
    function GetDanhMuc() {
        $scope.ListColumn = {};
        $scope.ListFlexValueSet = [];
        $scope.ListFlexValueParent = [];
        $scope.ListFlexValueCate = [];

        $.ajax({
            type: 'post',
            url: '/FlexValue/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                $scope.ListColumn = data.FlexValueColumns;
                $scope.ListFlexValueSet = data.FlexValueSets;
                $scope.ListFlexValueParent = data.FlexValueParents;
                $scope.ListFlexValueCate = data.FlexValueCates;

                $scope.$apply();
            }
        });
    }

    
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                FLEX_VALUE_SET_ID: {
                    required: true
                }
            },
            messages: {
                FLEX_VALUE_SET_ID: {
                    required: "Vui lòng chọn Danh mục"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/FlexValue/Add',
                data: { model: $scope.model},
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.GetShowHide = function (columnName) {
        var checkShow = $scope.ListColumn.filter(function (x) {
            return (x.FLEX_COLUMN_NAME == columnName);
        });

        if (checkShow != null && checkShow.length > 0) {
            return true;
        } else {
            return false;
        }
    };

    $scope.GetTitle = function (columnName) {
        var checkShow = $scope.ListColumn.filter(function (x) {
            return (x.FLEX_COLUMN_NAME == columnName);
        });

        if (checkShow != null && checkShow.length > 0) {
            return checkShow[0].END_USER_COLUMN_NAME;
        } else {
            return '';
        }
    };
    
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListColumn = [];
    $scope.ListFlexValueSet = [];
    $scope.ListFlexValueParent = [];
    $scope.ListFlexValueCate = [];

    $scope.model = {};
    angular.element(document).ready(function () {
        $scope.ListColumn = {};
        $scope.ListFlexValueSet = [];
        $scope.ListFlexValueParent = [];
        $scope.ListFlexValueCate = [];
        $.ajax({
            type: 'post',
            url: '/FlexValue/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.ListColumn = data.FlexValueColumns;
                    $scope.ListFlexValueSet = data.FlexValueSets;
                    $scope.ListFlexValueParent = data.FlexValueParents;
                    $scope.ListFlexValueCate = data.FlexValueCates;

                    $scope.model = data.data;
                    $scope.$apply();
                }
            }
        });
    });

    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                FLEX_VALUE_SET_ID: {
                    required: true
                }
            },
            messages: {
                FLEX_VALUE_SET_ID: {
                    required: "Vui lòng chọn Danh mục"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/FlexValue/Edit',
                data: { model: $scope.model },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.GetShowHide = function (columnName) {

        var checkShow = $scope.ListColumn.filter(function (x) {
            return (x.FLEX_COLUMN_NAME == columnName);
        });

        if (checkShow != null && checkShow.length > 0) {
            return true;
        } else {
            return false;
        }
    };

    $scope.GetTitle = function (columnName) {

        var checkShow = $scope.ListColumn.filter(function (x) {
            return (x.FLEX_COLUMN_NAME == columnName);
        });

        if (checkShow != null && checkShow.length > 0) {
            return checkShow[0].END_USER_COLUMN_NAME;
        } else {
            return '';
        }
    };
});