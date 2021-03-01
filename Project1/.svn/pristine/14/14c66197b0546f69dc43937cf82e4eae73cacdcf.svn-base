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

    $scope.donvitinh = [
        { 'ID': 1, 'NAME': 'Chiếc' },
        { 'ID': 2, 'NAME': 'Đôi' },
        { 'ID': 3, 'NAME': 'Bộ' },
    ]

    $scope.nhomhang = [
        { 'ID': 1, 'NAME': 'Nhom 1' },
        { 'ID': 2, 'NAME': 'Nhom 2' },
        { 'ID': 3, 'NAME': 'Nhom 3' },
    ]

    $scope.loaihang = [
        { 'ID': 1, 'NAME': 'Loại 1' },
        { 'ID': 2, 'NAME': 'Loại 2' },
        { 'ID': 3, 'NAME': 'Loại 3' },
    ]

    $scope.trangthai = [
        {'MA': 'N', 'NAME': 'Khởi tạo' },
        {'MA': 'A', 'NAME': 'Đã duyệt' }
    ];
    $scope.index = -1;
    $scope.ID_NHAP_XUAT = 0;

    $scope.modelSearch = {
        SO_KE_HOACH: '',
        DON_VI_NHAP_NHAN_ID: '',
        NGUON_TIEP_NHAN_ID: '',
        NGUOI_NHAP_NHAN_ID: '',
        TU_NGAY: '',
        DEN_NGAY: '',
        TRANG_THAI: '',
        OrderByClause: '',
        maxSize: 5,
        pageSize: 10,
        currentPage: 1,
        totalItems: 0
    }

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

    $scope.hang = {
        ID_NHAP_XUAT: '',
        SO_TT: '',
        ID_KHO: '',
        ID_THIET_BI: '',
        HANG_HOA: '',
        SO_LUONG: '',
        ID_CHAT_LUONG: '',
        GHI_CHU: '',
        NGUON_GOC: '',
        NHA_SAN_XUAT: '',
        NAM_SAN_XUAT: ''
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
                    toastr.error(response.Title);
                } else {

                    $scope.dsNguoiNhan = response.User;
                    $scope.dsDonVi = response.Units;
                    $scope.dmHang = response.dsHang;
                    $scope.fulldmHang = response.dsHang;
                }

                $scope.$apply();
            }
        });   
    });
   
    nhapkho();
    $scope.TimKiem = function () {
        nhapkho();
    }
    $scope.pageChanged = function () {
        nhapkho();
    };
    function nhapkho() {
        showToast();
        $scope.modelSearch.TU_NGAY = DateString($scope.modelSearch.TU_NGAY);
        $scope.modelSearch.DEN_NGAY = DateString($scope.modelSearch.DEN_NGAY);
        $scope.dsNhapKho = [];
        $scope.ID_NHAP_XUAT = 0;
        $scope.stt = 0;
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/dsNhapKho',
            data: $scope.modelSearch,
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsNhapKho = data.data;
                    if (data.data.length > 0) {
                        $scope.ID_NHAP_XUAT = data.data[0].ID_NHAP_XUAT;
                        $scope.modelSearch.totalItems = data.data[0].TotalRow;
                    }
                }
                $scope.modelSearch.TU_NGAY = StringDate($scope.modelSearch.TU_NGAY);
                $scope.modelSearch.DEN_NGAY = StringDate($scope.modelSearch.DEN_NGAY);
                
                $scope.$apply();
                hideLoading();
            }
        });   
                                
    };

    $scope.ChangeHang = function (id) {
        angular.forEach($scope.dmHang, function (data) {
          
            if (data.ID_THIET_BI == id) {
                
                $scope.hang.DON_VI_TINH = data.DON_VI_TINH;
            }
        });
    }

    $scope.changeLoaiHang = function (hang) {
        if (hang != '' && hang != null) {
            $scope.dmHang = $scope.dmHang.filter(x => x.LOAI_HANG === hang);
        } else {
            $scope.dmHang = $scope.fulldmHang;
        }
        
    }
    $scope.changeNhomHang = function (hang) {
        console.log(hang);
        if (hang != '' && hang != null) {
            $scope.dmHang = $scope.dmHang.filter(x => x.NHOM_HANG === hang);
        } else {
            $scope.dmHang = $scope.fulldmHang;
        }

    }
    

    $scope.chiTiet = function (item, index) {
        $scope.ID_NHAP_XUAT = item.ID_NHAP_XUAT;
    }
    $scope.order_lh = '';
    $scope.ORDER_KH = function (evan) {
        console.log(evan.currentTarget.classList);
        if ($scope.order_lh == '') {
            $("#or_skh").addClass("fa fa-sort-amount-down-alt");
            $scope.order_lh = 'ASC';
            $scope.modelSearch.OrderByClause = 'SO_KE_HOACH ASC';
        } else if ($scope.order_lh == 'ASC') {
            $("#or_skh").removeClass("fa fa-sort-amount-down-alt");
            $("#or_skh").addClass("fa fa-sort-amount-down");
            $scope.order_lh = 'DESC';
            $scope.modelSearch.OrderByClause = 'SO_KE_HOACH DESC';
        } else {
            $("#or_skh").removeClass("fa fa-sort-amount-down");
            $scope.order_lh = '';
            $scope.modelSearch.OrderByClause = '';
        }
        $("#or_ntn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ntn").removeClass("fa fa-sort-amount-down");
        $("#or_dv").removeClass("fa fa-sort-amount-down-alt");
        $("#or_dv").removeClass("fa fa-sort-amount-down");
        $("#or_nn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_nn").removeClass("fa fa-sort-amount-down");
        $("#or_ng").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ng").removeClass("fa fa-sort-amount-down");
        nhapkho();
    }

    $scope.order_ntn = '';
    $scope.ORDER_NTN = function () {
        if ($scope.order_ntn == '') {
            $("#or_ntn").addClass("fa fa-sort-amount-down-alt");
            $scope.order_ntn = 'ASC';
            $scope.modelSearch.OrderByClause = 'NGUON_TIEP_NHAN_ID ASC';
        } else if ($scope.order_ntn == 'ASC') {
            $("#or_ntn").removeClass("fa fa-sort-amount-down-alt")
            $("#or_ntn").addClass("fa fa-sort-amount-down");
            $scope.order_ntn = 'DESC';
            $scope.modelSearch.OrderByClause = 'NGUON_TIEP_NHAN_ID DESC';
        } else {
            $("#or_ntn").removeClass("fa fa-sort-amount-down");
            $scope.order_ntn = '';
            $scope.modelSearch.OrderByClause = '';
        }
        $("#or_skh").removeClass("fa fa-sort-amount-down-alt");
        $("#or_skh").removeClass("fa fa-sort-amount-down");
        $("#or_dv").removeClass("fa fa-sort-amount-down-alt");
        $("#or_dv").removeClass("fa fa-sort-amount-down");
        $("#or_nn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_nn").removeClass("fa fa-sort-amount-down");
        $("#or_ng").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ng").removeClass("fa fa-sort-amount-down");
        nhapkho();
    }

    $scope.order_dv = '';
    $scope.ORDER_DV = function () {
        if ($scope.order_dv == '') {
            $("#or_dv").addClass("fa fa-sort-amount-down-alt");
            $scope.order_dv = 'ASC';
            $scope.modelSearch.OrderByClause = 'DON_VI_NHAP_NHAN ASC';
        } else if ($scope.order_dv == 'ASC') {
            $("#or_dv").removeClass("fa fa-sort-amount-down-alt");
            $("#or_dv").addClass("fa fa-sort-amount-down");
            $scope.order_dv = 'DESC';
            $scope.modelSearch.OrderByClause = 'DON_VI_NHAP_NHAN DESC';
        } else {
            $("#or_dv").removeClass("fa fa-sort-amount-down");
            $scope.order_dv = '';
            $scope.modelSearch.OrderByClause = '';
        }
        $("#or_skh").removeClass("fa fa-sort-amount-down-alt");
        $("#or_skh").removeClass("fa fa-sort-amount-down");
        $("#or_ntn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ntn").removeClass("fa fa-sort-amount-down");
        $("#or_nn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_nn").removeClass("fa fa-sort-amount-down");
        $("#or_ng").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ng").removeClass("fa fa-sort-amount-down");
        nhapkho();
    }

    $scope.order_nn = '';
    $scope.ORDER_NN = function () {
        if ($scope.order_nn == '') {
            $("#or_nn").addClass("fa fa-sort-amount-down-alt");
            $scope.order_nn = 'ASC';
            $scope.modelSearch.OrderByClause = 'NGUOI_NHAP_NHAN ASC';
        } else if ($scope.order_nn == 'ASC') {
            $("#or_nn").removeClass("fa fa-sort-amount-down-alt");
            $("#or_nn").addClass("fa fa-sort-amount-down");
            $scope.order_nn = 'DESC';
            $scope.modelSearch.OrderByClause = 'NGUOI_NHAP_NHAN DESC';
        } else {
            $("#or_nn").removeClass("fa fa-sort-amount-down");
            $scope.order_nn = '';
            $scope.modelSearch.OrderByClause = '';
        }
        $("#or_skh").removeClass("fa fa-sort-amount-down-alt");
        $("#or_skh").removeClass("fa fa-sort-amount-down");
        $("#or_ntn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ntn").removeClass("fa fa-sort-amount-down");
        $("#or_dv").removeClass("fa fa-sort-amount-down-alt");
        $("#or_dv").removeClass("fa fa-sort-amount-down");
        $("#or_ng").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ng").removeClass("fa fa-sort-amount-down");
        nhapkho();
    }

    $scope.order_ng = '';
    $scope.ORDER_NG = function () {
        if ($scope.order_ng == '') {
            $("#or_ng").addClass("fa fa-sort-amount-down-alt");
            $scope.order_ng = 'ASC';
            $scope.modelSearch.OrderByClause = 'NGAY_NHAP_NHAN ASC';
        } else if ($scope.order_ng == 'ASC') {
            $("#or_ng").removeClass("fa fa-sort-amount-down-alt");
            $("#or_ng").addClass("fa fa-sort-amount-down");
            $scope.order_ng = 'DESC';
            $scope.modelSearch.OrderByClause = 'NGAY_NHAP_NHAN DESC';
        } else {
            $("#or_ng").removeClass("fa fa-sort-amount-down");
            $scope.order_ng = '';
            $scope.modelSearch.OrderByClause = '';
        }
        $("#or_skh").removeClass("fa fa-sort-amount-down-alt");
        $("#or_skh").removeClass("fa fa-sort-amount-down");
        $("#or_ntn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_ntn").removeClass("fa fa-sort-amount-down");
        $("#or_dv").removeClass("fa fa-sort-amount-down-alt");
        $("#or_dv").removeClass("fa fa-sort-amount-down");
        $("#or_nn").removeClass("fa fa-sort-amount-down-alt");
        $("#or_nn").removeClass("fa fa-sort-amount-down");
        nhapkho();
    }

    $scope.add = function () {
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
        $scope.ListHangHoa = [];
        $('#nhapKho').modal('show');
    }

    $scope.themHang = function () {
        $scope.index = -1;
        $scope.hang = {
            ID_NHAP_XUAT: '',
            SO_TT: '',
            ID_KHO: '',
            ID_THIET_BI: '',
            HANG_HOA: '',
            SO_LUONG: '',
            ID_CHAT_LUONG: '',
            GHI_CHU: '',
            NGUON_GOC: '',
            NHA_SAN_XUAT: '',
            NAM_SAN_XUAT: ''
        }
        $('#nhapHang').modal('show');
    }

    $scope.luuHang = function () {
        $scope.hh = {};
        console.log($scope.index);
        if ($scope.index != -1) {
            $scope.ListHangHoa[$scope.index].ID_KHO = $scope.hang.ID_KHO;
            $scope.ListHangHoa[$scope.index].ID_THIET_BI = $scope.hang.ID_THIET_BI;
            angular.forEach($scope.dmHang, function (data) {
                if (data.ID_THIET_BI == $scope.hang.ID_THIET_BI) {
                    //$scope.hh.HANG_MA = data.MA;
                    $scope.ListHangHoa[$scope.index].HANG_HOA = data.TEN;
                }
            })
            $scope.ListHangHoa[$scope.index].SO_LUONG = $scope.hang.SO_LUONG;
            $scope.ListHangHoa[$scope.index].ID_CHAT_LUONG = $scope.hang.ID_CHAT_LUONG;
            $scope.ListHangHoa[$scope.index].GHI_CHU = $scope.hang.GHI_CHU;
            $scope.ListHangHoa[$scope.index].NGUON_GOC = $scope.hang.NGUON_GOC;
            $scope.ListHangHoa[$scope.index].NHA_SAN_XUAT = $scope.hang.NHA_SAN_XUAT;
            $scope.ListHangHoa[$scope.index].NAM_SAN_XUAT = $scope.hang.NAM_SAN_XUAT;

        } else {
            $scope.hh.ID_KHO = $scope.hang.ID_KHO;
            //angular.forEach($scope.dsKho, function (data) {
            //    if (data.ID == $scope.hang.ID_KHO) {
            //        $scope.hh.MA_KHO = data.MA;
            //        $scope.hh.TEN_KHO = data.NAME;
            //    }
            //})

            $scope.hh.ID_THIET_BI = $scope.hang.ID_THIET_BI;
            angular.forEach($scope.dmHang, function (data) {
                if (data.ID_THIET_BI == $scope.hang.ID_THIET_BI) {
                    //$scope.hh.HANG_MA = data.MA;
                    $scope.hh.HANG_HOA = data.TEN;
                }
            })

            $scope.hh.SO_LUONG = $scope.hang.SO_LUONG;
            $scope.hh.ID_CHAT_LUONG = $scope.hang.ID_CHAT_LUONG;
            //angular.forEach($scope.chatLuong, function (data) {
            //    if (data.ID == $scope.hang.CHAT_LUONG) {
            //        $scope.hh.TEN_CHAT_LUONG = data.NAME;
            //    }
            //})

            $scope.hh.GHI_CHU = $scope.hang.GHI_CHU;
            $scope.hh.NGUON_GOC = $scope.hang.NGUON_GOC;
            $scope.hh.NHA_SAN_XUAT = $scope.hang.NHA_SAN_XUAT;
            $scope.hh.NAM_SAN_XUAT = $scope.hang.NAM_SAN_XUAT;
            $scope.ListHangHoa.push($scope.hh);
        }
        
       
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
                    toastr.error(data.Title);
                } else {
                    //$scope.ListHangHoa = [];
                    toastr.success(data.Title);
                    nhapkho();
                    
                }
                $scope.model.NGAY_NHAP_NHAN = StringDate($scope.model.NGAY_NHAP_NHAN);
                hideLoading();
                $scope.$apply();
            }
        });   
    }

    $scope.edit = function () {
        $.ajax({
            type: 'post',
            url: '/Nhap_Kho/laydlSua',
            data: { id: $scope.ID_NHAP_XUAT},
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    console.log(data);
                    if (data.Nhap.length > 0) {
                        $scope.model = data.Nhap[0];
                        $scope.model.NGAY_NHAP_NHAN = StringDate($scope.model.NGAY_NHAP_NHAN);
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

    $scope.editct = function (item, index) {
        $scope.index = index;
        console.log(item);
        $scope.hang = item;
        $('#nhapHang').modal('show');

    }

    $scope.deletect = function (index) {
        $scope.ListHangHoa.splice(index, 1);
    }

   
    $scope.delete = function () {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa bản ghi đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/Nhap_Kho/xoa',
                            data: { Id: $scope.ID_NHAP_XUAT },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    nhapkho();
                                    toastr.success(data.Title);
                                    
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
            date = date.substring(6, 10) + date.substring(0, 2) + date.substring(3, 5);
        }
        return date;
    }

    //format String YYYYMMDD to DD/MM/YYYY
    function StringDate(date) {
        if (date != undefined && date != "") {
            date = date.substring(4, 6) + '/' + date.substring(6, 8) + '/' + date.substring(0, 4);
        }
        return date;
    }
});

