(function () {
    'use strict';

    angular
        .module('app')
        .factory('guidService', guidService);

    guidService.$inject = ['$http'];

    function guidService($http) {

        var guid = undefined;

        var service = {
            getGuid: getGuid
        };

        return service;

        function getGuid() {

            return $http({
                method: 'GET',
                url: '/api/guid'
            });

        }
    }
})();