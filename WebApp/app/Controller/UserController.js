app.controller("UserController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "FULL_NAME DESC";
    $scope.ListUserGroup = [];
    $scope.ListUser = [];

    angular.element(document).ready(function () {
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;
    $scope.RoleBtnLock = false;
    $scope.RoleBtnUnlock = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/User/GetBottomAction',
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

                        if (item == 'btnLock') {
                            $scope.RoleBtnLock = true;
                        }
                        if (item == 'btnUnlock') {
                            $scope.RoleBtnUnlock = true;
                        }
                    });
                }
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
            url: '/User/GetListUser',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListUser = data.data;
                $scope.$apply();
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
            templateUrl: '/User/_Add',
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
            templateUrl: '/User/_Edit',
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.delete = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa tài khoản ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/User/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
                                }
                                hideLoading();
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

    $scope.lockUser = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/User/_LockUser',
            controller: 'lockUser',
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

    $scope.unlockUser = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn mở khóa tài khoản ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/User/UnLockUser',
                            data: { userId: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
                                }
                                hideLoading();
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

});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ph_numbr = /(09|01[2|6|8|9])+([0-9]{8})\b/;
    $scope.ListUserGroup = [];
    $scope.ListRole = [];
    $scope.ListUnit = [];
    $scope.FileName = "";
    $scope.ListSCOPE = [{ ID: 1, Name: 'Chức danh 1' }, { ID: 2, Name: 'Chức danh 2' }];
    $scope.ListLEVEL = [{ ID: 1, Name: 'Cấp bậc 1' }, { ID: 2, Name: 'Cấp bậc 2' }];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    function GetDanhMuc() {
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.ListUnit = data.Units;
                $scope.RoleId = $scope.ListUserGroup[0].ID;
                $scope.model.UNIT_ID = $scope.ListUnit[0].UNIT_ID;
                $scope.model.Status = true;
                $scope.$apply();
                hideLoading();
            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        showToast();
        $("#formSubmit").validate({
            rules: {
                LOGIN_NAME: {
                    required: true,
                    maxlength: 30
                },
                //RoleId: {
                //    required: true
                //},
                FULL_NAME: {
                    required: true,
                    maxlength: 150
                },
                UNIT_ID: {
                    required: true
                },
                USER_GROUP: {
                    required: true
                },
                USER_CLASS: {
                    required: true
                }

            },
            messages: {
                LOGIN_NAME: {
                    required: "Vui lòng nhập Tên truy cập",
                    maxlength: "Tên truy cập không được vượt quá 30 ký tự"
                },
                //RoleId: {
                //    required: "Vui lòng chọn Nhóm quyền"
                //},
                FULL_NAME: {
                    required: "Vui lòng nhập Họ và tên",
                    maxlength: "Họ và tên không được vượt quá 150 ký tự"
                },
                UNIT_ID: {
                    required: "Vui lòng chọn Đơn vị"
                },
                USER_GROUP: {
                    required: "Vui lòng chọn Nhóm người dùng"
                },
                USER_CLASS: {
                    required: "Vui lòng chọn Cấp"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var roleId = "";

            if ($scope.ListRole == null || $scope.ListRole.length == 0) {
                toastr.error("Bạn chưa chọn quyền cho người dùng.");
                return;
            }
            else {
                for (var i = 0; i < $scope.ListRole.length; i++) {
                    if (roleId == null || roleId == "")
                        roleId = $scope.ListRole[i];
                    else
                        roleId += ","+ $scope.ListRole[i];
                }
            }

            if ($scope.model.START_DATE != null || $scope.model.START_DATE != "") {
                $scope.model.START_DATE = moment($scope.model.START_DATE).format("YYYYMMDD");
            }
            if ($scope.model.END_DATE != null || $scope.model.END_DATE != "") {
                $scope.model.END_DATE = moment($scope.model.END_DATE).format("YYYYMMDD");
            }

            $.ajax({
                type: 'post',
                url: '/User/Add',
                data: { user: $scope.model, fileName: $scope.FileName, roleId: roleId },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                    hideLoading();
                }
            });
        }
    };


    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.IMG_PATH = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.IMG_PATH = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListUserGroup = [];
    $scope.ListUnit = [];
    $scope.ListRole = [];
    $scope.FileName = "";
    $scope.model = {};
    $scope.ListTITLE = [{ ID: 1, Name: 'Chức danh 1' }, { ID: 2, Name: 'Chức danh 2' }];
    $scope.ListLEVEL = [{ ID: 1, Name: 'Cấp bậc 1' }, { ID: 2, Name: 'Cấp bậc 2' }];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        $scope.ListRole = [];
        $.ajax({
            type: 'post',
            url: '/User/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;

                    
                    //$scope.ListRole = data.RoleId;
                    if (data.START_DATE != null || data.START_DATE != "") {
                        $scope.model.START_DATE = moment(data.START_DATE).format("MM/DD/YYYY");
                    }
                    if (data.END_DATE != null || data.END_DATE != "") {
                        $scope.model.END_DATE = moment(data.END_DATE).format("MM/DD/YYYY");
                    }
                    if ($scope.model.IMG_PATH != null || $scope.model.IMG_PATH != "") {
                        ConvertImageToBase64($scope.model.IMG_PATH);
                    }

                    for (var j = 0; j < data.RoleId.length; j++) {
                        $scope.ListRole.push(data.RoleId[j]);
                    }
                    
                    $scope.$apply();
                    hideLoading();
                }
            }
        });
    });


    function GetDanhMuc() {
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.Roles;
                $scope.ListUnit = data.Units;
                $scope.model.Status = true;
                $scope.$apply();
                hideLoading();
            }
        });
    }
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                LOGIN_NAME: {
                    required: true,
                    maxlength: 30
                },
                //RoleId: {
                //    required: true
                //},
                FULL_NAME: {
                    required: true,
                    maxlength: 150
                },
                UNIT_ID: {
                    required: true
                },
                USER_GROUP: {
                    required: true
                },
                USER_CLASS: {
                    required: true
                }

            },
            messages: {
                LOGIN_NAME: {
                    required: "Vui lòng nhập Tên truy cập",
                    maxlength: "Tên truy cập không được vượt quá 30 ký tự"
                },
                //RoleId: {
                //    required: "Vui lòng chọn Nhóm quyền"
                //},
                FULL_NAME: {
                    required: "Vui lòng nhập Họ và tên",
                    maxlength: "Họ và tên không được vượt quá 150 ký tự"
                },
                UNIT_ID: {
                    required: "Vui lòng chọn Đơn vị"
                },
                USER_GROUP: {
                    required: "Vui lòng chọn Nhóm người dùng"
                },
                USER_CLASS: {
                    required: "Vui lòng chọn Cấp"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var roleId = "";

            if ($scope.ListRole == null || $scope.ListRole.length == 0) {
                toastr.error("Bạn chưa chọn quyền cho người dùng.");
                return;
            }
            else {
                for (var i = 0; i < $scope.ListRole.length; i++) {
                    if (roleId == null || roleId == "")
                        roleId = $scope.ListRole[i];
                    else
                        roleId += "," + $scope.ListRole[i];
                }
            }
            
            if ($scope.model.START_DATE != null || $scope.model.START_DATE != "") {
                if ($scope.model.START_DATE.indexOf("/") >= 0) {
                    var array = $scope.model.START_DATE.split('/');
                    $scope.model.START_DATE = array[2] + array[1] + array[0] ;
                }
                else {
                    $scope.model.START_DATE = moment($scope.model.START_DATE).format("YYYYMMDD");
                }
            }
            if ($scope.model.END_DATE != null || $scope.model.END_DATE != "") {
                if ($scope.model.END_DATE.indexOf("/") >= 0) {
                    var array = $scope.model.END_DATE.split('/');
                    $scope.model.END_DATE = array[2] + array[1] + array[0];
                }
                else {
                    $scope.model.END_DATE = moment($scope.model.END_DATE).format("YYYYMMDD");
                }
            }

            $.ajax({
                type: 'post',
                url: '/User/Edit',
                data: { user: $scope.model, fileName: $scope.FileName, roleId: roleId },
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

    $scope.SelectFile = function (e) {
        $scope.model.IMG_PATH = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.IMG_PATH = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };

    //$scope.ChangeEmployee = function () {

    //    var documentCheck = $scope.ListEmployee.filter(function (x) {
    //        return (x.Id == $scope.model.EmployeeId);
    //    });
    //    if (documentCheck != null && documentCheck.length > 0) {
    //        $scope.model.Name = documentCheck[0].FullName;
    //        $scope.model.Phone = documentCheck[0].Phone;
    //        $scope.model.Email = documentCheck[0].Email;
    //        if (documentCheck[0].AvatarPath != null && documentCheck[0].AvatarPath != "") {
    //            $.ajax({
    //                type: 'POST',
    //                dataType: 'json',
    //                cache: false,
    //                async: true,
    //                url: '/User/ConvertPathImageToBase64',
    //                data: {
    //                    path: documentCheck[0].AvatarPath
    //                },
    //                success: function (response) {
    //                    var base64File = "";
    //                    if (response.data != null && response.data != "") {
    //                        $scope.model.IMG_PATH = response.data;
    //                        base64File = 'data:image/jpeg;base64,' + response.data;
    //                    }
    //                    else {
    //                        base64File = '/Content/Images/noimg.png';
    //                    }
    //                    $('#pathPhoto').attr('src', base64File);
    //                }
    //                , error: function (xhr) {
    //                }
    //            });
    //        }
    //    }
    //};

    //type: 1 là create, 2 là update, 3 là detail
    function ConvertImageToBase64(path) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: true,
            url: '/User/ConvertPathImageToBase64',
            data: {
                path: path
            },
            success: function (response) {
                var base64File = "";
                if (response.data != null && response.data != "") {
                    base64File = 'data:image/jpeg;base64,' + response.data;
                }
                else {
                    base64File = '/Content/Images/noimg.png';
                }
                $('#pathPhoto').attr('src', base64File);
            }
            , error: function (xhr) {
            }
        });
    }
});

app.controller('lockUser', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.Content = "";
    $scope.ItemId = 0;

    angular.element(document).ready(function () {
        $scope.ItemId = itemId;
    });

    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                
            },
            messages: {
                
            }
        });
        if ($("#formSubmit").valid()) {
            if ($scope.Content == null || $scope.Content =="") {
                toastr.error("Bạn chưa nhập ý kiến khóa người dùng.");
                return;
            }
            $.ajax({
                type: 'post',
                url: '/User/LockUser',
                data: { userId: $scope.ItemId, content: $scope.Content },
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
   
});