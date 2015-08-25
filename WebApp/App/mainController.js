(function () {
    'use strict';

    angular
        .module('app')
        .controller('mainController', mainController);

    mainController.$inject = ['$location', '$http', 'messageProxy', '$scope'];

    function mainController($location, $http, messageProxy, $scope) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'mainController';
        vm.items = [];
        vm.message = "";

        vm.currentItem = {};
        vm.selectItem = function (item) {
            angular.copy(item, vm.currentItem);
            //vm.currentItem = item;
        }

        vm.addItem = function () {
            $http({
                method: 'POST',
                url: '/api/test',
                data: vm.currentItem
            }).then(function (response) {
            }, function (error) {
            });
        }

        vm.updateItem = function () {
            $http({
                method: 'PUT',
                url: '/api/test',
                data: vm.currentItem
            }).then(function (response) {
            }, function (error) {
            });
        }

        var init = function () {
            messageInitializer();
            getItems();
        }

        var getItems = function () {
            $http({
                method: 'GET',
                url: '/api/test'
            }).then(function (response) {
                vm.items = response.data;
            }, function (error) {
                var a = 1;
            });
        }

        var messageInitializer = function () {
            $scope.$on('MESSAGE_RECEIVED', function (event, data) {

                if (data.message.Success == true)
                    toastr.success(data.message.Message);
                if (data.message.Success == false)
                    toastr.error(data.message.Message);

                $scope.$apply(function () {
                    getItems();
                });

            });

            $scope.$on('MESSAGE_RECEIVED_GLOBAL', function (event, data) {
                $scope.$apply(function () {
                    getItems();
                });

            });


            $scope.$on('MESSAGE_CONNECTION_STARTED', function (event, data) {
                toastr.info("Message connection started", "Message connection started");
            });
        }


        init();

    }
})();
