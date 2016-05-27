import 'angular';
import 'angular-i18n/angular-locale_de';
var C = (function () {
    function C($http) {
        var _this = this;
        $http.get('api/values').then(function (res) {
            _this.time = res.data;
        });
    }
    C.$inject = ['$http'];
    return C;
}());
angular.module('m', []).controller('c', C);
