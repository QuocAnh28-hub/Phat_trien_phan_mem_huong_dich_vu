 var app = angular.module('AppBanHang', []);
app.controller("HomeCtrl", function ($scope, $http) {
    $scope.listSanPhamMoi;  
    $scope.LoadSanPhamMoi = function () {		 
        $http({
            method: 'GET', 
            url: current_url + '/api/Home/get-moi/10',
        }).then(function (response) {			 
            $scope.listSanPhamMoi = response.data;
			makeScript('js/main.js')
        });
    };  
});

