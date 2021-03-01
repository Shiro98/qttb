app.controller("ChotQuanSoDauKyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.AddFileChotQuanSoDauKy = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChotQuanSoDauKy/_AddFileChotQuanSoDauKy',
            controller: 'addFileChotQuanSoDauKy',
            size: 'xs',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };

    var ListUnit = [];

    //#region Danh sách Cán bộ công tác
    $scope.JxlCbCongTac = {};
    $scope.JxlCbCongTac.data = [];
    $scope.ID_NHAP_XUAT = 26; //dữ liệu demo

    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_CbCongTac',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_MU, val.CO_GIAY, val.CO_QA, val.DANG_KY_HV]
                    $scope.JxlCbCongTac.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });
    $scope.JxlCbCongTac.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlCbCongTac.column = [
        { type: 'text', width: '0'},
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '100' }
    ];
    $scope.JxlCbCongTac.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Không nhận hiện vật ', rowspan: '2' }

        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' }
        ]
    ];

    $scope.JxlCbCongTac.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '']];
    $scope.JxlCbCongTac.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value + " - " + cellName);
    };
    //#endregion 
    //#region Danh sách cán bộ chờ hưu 
    $scope.JxlCbChoHuu = {};
    $scope.JxlCbChoHuu.data = [];
     
    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_CbChoHuu',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                    $scope.JxlCbChoHuu.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });
     

    $scope.JxlCbChoHuu.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlCbChoHuu.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlCbChoHuu.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlCbChoHuu.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];

    $scope.JxlCbChoHuu.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };
     
    //#endregion
    //#region Danh sách cán bộ biệt phái    
    $scope.JxlBietPhai = {};
    $scope.JxlBietPhai.data = [];
       
    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_BietPhai',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                    $scope.JxlBietPhai.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });
     

    $scope.JxlBietPhai.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlBietPhai.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlBietPhai.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlBietPhai.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];


    $scope.JxlBietPhai.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };    
    //#endregion
    //#region Danh sách cán bộ đi học 
    $scope.JxlCbDiHoc = {};
    $scope.JxlCbDiHoc.data = [];
     
    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_CbDiHoc',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                    $scope.JxlCbDiHoc.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });

    $scope.JxlCbDiHoc.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlCbDiHoc.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlCbDiHoc.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlCbDiHoc.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];
    
    $scope.JxlCbDiHoc.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };

    //#endregion
    //#region Danh sách chiến sỹ nghĩa vụ
    $scope.JxlCsNghiaVu = {};
    $scope.JxlCsNghiaVu.data = []; 

    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_CsNghiaVu',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA]
                    $scope.JxlCsNghiaVu.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });
    $scope.JxlCsNghiaVu.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];
    $scope.JxlCsNghiaVu.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' }
    ];
    $scope.JxlCsNghiaVu.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' }

        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' }
        ]
    ];

    $scope.JxlCsNghiaVu.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '']];
    $scope.JxlCsNghiaVu.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };
    //#endregion
    //#region Danh sách tuyển mới, phong hàm
    $scope.JxlTuyenMoiPhHam = {};
    $scope.JxlTuyenMoiPhHam.data = [];

    $.ajax({
        type: 'post',
        async: false,
        url: '/ChotQuanSoDauKy/DanhSachTp_TuyenMoi',
        data: { txnId: $scope.ID_NHAP_XUAT },
        success: function (response) {
            if (response.Error) {
                toastr.error(response.Title);
            } else {
                angular.forEach(response.data, function (val, key) {
                    $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.NGANH_NGOAI, val.TRUONG_CAND, val.TBINH_TUYEN_LAI]
                    $scope.JxlTuyenMoiPhHam.data.push($scope.ItemJexcel);
                });
            }
            $scope.$apply();
        }
    });
    $scope.JxlTuyenMoiPhHam.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J','K','L','M'];
    $scope.JxlTuyenMoiPhHam.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' }
    ];
    $scope.JxlTuyenMoiPhHam.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Tuyển mới CBCS (phong hàm lần đầu) ', colspan: '3' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Ngành ngoài' },
            { title: 'Trường CAND' },
            { title: 'T.binh tuyển lại'}
        ]
    ];

    $scope.JxlTuyenMoiPhHam.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '', '']];
    $scope.JxlTuyenMoiPhHam.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };
    //#endregion
    var SUMCOL = function (instance, columnId) {
        var total = 0;
        for (var j = 0; j < instance.options.data.length; j++) {
            if (Number(instance.records[j][columnId - 1].innerHTML)) {
                total += Number(instance.records[j][columnId - 1].innerHTML.replace('.', ''));
            }
        }
        return total;
    }

    $.ajax({
        type: 'post',
        url: '/User/GetDanhMuc',
        data: {},
        success: function (data) {
            angular.forEach(data.Units, function (val, key) {
                ListUnit.push({ id: val.UNIT_ID, name: val.UNIT_NAME });
            });
        }
    });

    $scope.export = function () {
        window.location.href = "/Home/ExportExcelCustom";
    }
});
app.controller('addFileChotQuanSoDauKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};

    $scope.Upload = function () {
        var fdata = new FormData();
        var fileInput = $('.custom-file-input')[0];
        for (i = 0; i < fileInput.files.length; i++) {
            //Appending each file to FormData object
            fdata.append(fileInput.files[i].name, fileInput.files[i]);
        }

        $.ajax({
            type: 'post',
            url: '/ChotQuanSoDauKyController/ImportFileExel',
            data: fdata,
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
    $scope.cancel = function () {
        $uibModalInstance.close();
    };


});
