app.controller("Don_vi_tinhController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modell = 26;
    $scope.List = [];
    $scope.modelSearch.currentPage = 1;
    angular.element(document).ready(function () {
        $scope.LoadPage();
    });
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/Don_vi_tinh/DanhSach',
            data: { tID: $scope.modell },
            success: function (data) {
                $scope.List = data.data;
            }
        });
    };
});