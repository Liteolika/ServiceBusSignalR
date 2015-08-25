(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        //'ngRoute'

        // Custom modules 
        //'guidService'
        // 3rd Party Modules
        'angularUUID2'
    ]);

    //app.constant('CONFIG', {
    //    TEST: 'HEJ',
    //    APPID: ''
    //});

    //app.run(['CONFIG', 'uuid2', '$http', function (CONFIG, uuid2, $http) {
    //    CONFIG.APPID = uuid2.newguid();
    //    $http.defaults.headers.common['X-ClientID'] = CONFIG.APPID;
    //}]);

    //app.config(['$httpProvider', 'CONFIG', function ($httpProvider, CONFIG) {
    //    //$httpProvider.defaults.headers.common['X-ClientID'] = CONFIG.APPID;
    //}]);

})();