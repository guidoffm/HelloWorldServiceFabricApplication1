/// <reference path="typings/angularjs/angular.d.ts" />
import 'angular';
import 'angular-i18n/angular-locale_de';

class C {
    static $inject = ['$http'];
    constructor($http) {
        $http.get('api/values').then(res => {
            this.time = res.data;
        });
    }

    time;
}


angular.module('m', []).controller('c', C);
