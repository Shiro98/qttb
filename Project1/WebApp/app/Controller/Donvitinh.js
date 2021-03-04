app.controller("Don_vi_tinhController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.DanhSach = {
        ID: '',
        TEN_DVT: '',
        MO_TA: ''
    }
    angular.element(document).ready(function () {
        $scope.LoadPage();
    });
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/Don_vi_tinh/DanhSach',
            data: {},
            success: function (data) {
                $scope.DanhSach = data.Data;

            }
        });
    };
    
});