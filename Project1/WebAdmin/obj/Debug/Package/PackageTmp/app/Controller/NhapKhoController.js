app.controller("NhapKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {

    $scope.nguonNhan = [
        { 'ID': 1, 'NAME': 'Công ty may 10' },
        { 'ID': 2, 'NAME': 'Công ty may 20' },
        { 'ID': 3, 'NAME': 'Công ty săn xuất oto' },
        { 'ID': 4, 'NAME': 'Công ty sản xuất haha' }
       
    ];
    $scope.chatLuong = [
        { 'ID': 1, 'NAME': 'Tốt' },
        { 'ID': 0, 'NAME': 'Không tốt' }
    ];
    $scope.dsKho = [
        { 'ID': 1, 'MA': 'K1', 'NAME': 'Kho 1' },
        { 'ID': 2, 'MA': 'K2','NAME': 'Kho 2' },
        { 'ID': 3, 'MA': 'K3','NAME': 'Kho 3' },
        { 'ID': 4, 'MA': 'K4','NAME': 'Kho 4' }

    ];

    $scope.model = {
        SO_KE_HOACH: '',
        SO_KE_HOACH_GAN: '',
        NGUON_TIEP_NHAN_ID: '',
        NGUOI_NHAP_NHAN_ID: '',
        NGUOI_NHAP_NHAN:'',
        NGAY_NHAP_NHAN: '',
        DON_VI_NHAP_NHAN_ID: '',
        DON_VI_NHAP_NHAN: '',
        LOAI: 1,
        GHI_CHU:''
    }
    $scope.dsNhapKho = [];
    $scope.dsNguoiNhan = [];
    $scope.dsDonVi = [];
    $scope.ListHangHoa = [];
    $scope.stt = 1;
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/GetDanhMuc',
            data: {},
            success: function (response) {
                console.log(response);
                if (response.Error) {
                    toastr.success(response.Title);
                } else {

                    $scope.dsNguoiNhan = response.User;
                    $scope.dsDonVi = response.Units;
                    $scope.dmHang = response.dsHang
                }

                $scope.$apply();
            }
        });   
    });

    nhapkho();
    function nhapkho() {
        $scope.dsNhapKho = [];
        $scope.stt = 0;
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/dsNhapKho',
            data: {},
            success: function (data) {
                if (data.Error) {
                    toastr.success(data.Title);
                } else {

                    $scope.dsNhapKho = data.data;
                    console.log($scope.dsNhapKho);

                }

                hideLoading();
                $scope.$apply();
            }
        });   
                                
    };

    $scope.add = function () {
        $scope.ListHangHoa = [];
        $scope.stt = 1;
        $('#nhapKho').modal('show');
    }

    $scope.themHang = function () {
        $('#nhapHang').modal('show');
    }

    $scope.luuHang = function () {
      
        $scope.hh = {};

        $scope.hh.SO_TT = $scope.stt;
        $scope.hh.ID_KHO = $scope.hang.ID_KHO;
        angular.forEach($scope.dsKho, function (data) {
            if (data.ID == $scope.hang.ID_KHO) {
                $scope.hh.MA_KHO = data.MA;
                $scope.hh.TEN_KHO = data.NAME;
            }
        })
        
        $scope.hh.ID_THIET_BI = $scope.hang.THIET_BI;
        angular.forEach($scope.dmHang, function (data) {
            if (data.ID_THIET_BI == $scope.hang.THIET_BI) {
                $scope.hh.HANG_MA = data.MA;
                $scope.hh.HANG_HOA = data.TEN;
            }
        })
        
        $scope.hh.SO_LUONG = $scope.hang.SO_LUONG;
        $scope.hh.ID_CHAT_LUONG = $scope.hang.CHAT_LUONG;
        angular.forEach($scope.chatLuong, function (data) {
            if (data.ID == $scope.hang.CHAT_LUONG) {
                $scope.hh.TEN_CHAT_LUONG = data.NAME;
            }
        })
        
        $scope.hh.GHI_CHU = $scope.hang.GHI_CHU;
        $scope.hh.NGUON_GOC = $scope.hang.NGUON_GOC;
        $scope.hh.NHA_SAN_XUAT = $scope.hang.NHA_SAN_XUAT;
        $scope.hh.NAM_SAN_XUAT = $scope.hang.NAM_SAN_XUAT;
        $scope.ListHangHoa.push($scope.hh);

        $scope.stt = $scope.stt + 1;
       
    }

    $scope.submit = function () {
        console.log($scope.ListHangHoa);
        angular.forEach($scope.dsNguoiNhan, function (data) {
            if (data.ID == $scope.model.NGUOI_NHAP_NHAN_ID) {
                $scope.model.NGUOI_NHAP_NHAN = data.NAME;
            }
        })

        angular.forEach($scope.dsDonVi, function (data) {
            if (data.UNIT_ID == $scope.model.DON_VI_NHAP_NHAN_ID) {
                $scope.model.DON_VI_NHAP_NHAN = data.UNIT_NAME;
            }
        })
        $scope.model.NGAY_NHAP_NHAN = DateString($scope.model.NGAY_NHAP_NHAN);
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/themMoi',
            data: { nhap: $scope.model, hang: $scope.ListHangHoa},
            success: function (data) {
                if (data.Error) {
                    toastr.success(data.Title);
                } else {
                    $scope.ListHangHoa = [];
                    $scope.stt = 1;
                    nhapkho();
                }

                hideLoading();
                $scope.$apply();
            }
        });   
    }

    $scope.edit = function (id) {
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/laydlSua',
            data: { id: id},
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    console.log(data);
                    if (data.Nhap.length > 0) {
                        $scope.model = data.Nhap[0];
                    } else {
                        $scope.model = {
                            SO_KE_HOACH: '',
                            SO_KE_HOACH_GAN: '',
                            NGUON_TIEP_NHAN_ID: '',
                            NGUOI_NHAP_NHAN_ID: '',
                            NGUOI_NHAP_NHAN: '',
                            NGAY_NHAP_NHAN: '',
                            DON_VI_NHAP_NHAN_ID: '',
                            DON_VI_NHAP_NHAN: '',
                            LOAI: 1,
                            GHI_CHU: ''
                        }
                    }
                    $scope.ListHangHoa = data.Nhap_ct;
                    $('#nhapKho').modal('show');
                }

                hideLoading();
                $scope.$apply();
            },
            error: function (error) {
                alert('error; ' + eval(error));
                alert('error; ' + error.responseText);
            }
        });   
    }
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


    //format String DD/MM/YYYY to YYYYMMDD
    function DateString(date) {
        if (date != undefined && date != "") {
            date = date.substring(6, 10) + date.substring(3, 5) + date.substring(0, 2);
        }
        return date;
    }

    //format String YYYYMMDD to DD/MM/YYYY
    function StringDate(date) {
        if (date != undefined && date != "") {
            date = date.substring(6, 8) + '/' + date.substring(4, 6) + '/' + date.substring(0, 4);
        }
        return date;
    }
});

