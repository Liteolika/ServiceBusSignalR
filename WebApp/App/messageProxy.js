(function () {
    'use strict';

    angular
        .module('app')
        .factory('messageProxy', messageProxy);

    messageProxy.$inject = ['$rootScope', '$http'];

    function messageProxy($rootScope, $http) {

        var messageHub = $.connection.messageHub;
        var clientid = undefined;

        var service = {
        };

        init();
        return service;

        function init() {
            startConnection();
            onRecieveMessage();
            onGlobalRecieveMessage();
        }

        function setIdentifier(id) {
            clientid = id;
            $http.defaults.headers.common['X-ClientID'] = id;
        }

        function startConnection() {
            $.connection.hub.start().done(function () {
                $rootScope.$broadcast('MESSAGE_CONNECTION_STARTED', { data: 'Started' });
            });
        }

        function onRecieveMessage() {
            messageHub.client.messageRecieved = function (message) {
                console.log(message.Message);
                var data = { message: message };
                $rootScope.$broadcast('MESSAGE_RECEIVED', data);
                $rootScope.$apply();
            };

            messageHub.client.identifierRecieved = function (identifier) {
                console.log("Got identifier: " + identifier);
                setIdentifier(identifier);
            };

        }

        function onGlobalRecieveMessage() {
            messageHub.client.globalMessageRecieved = function (message) {
                var data = { message: message };
                $rootScope.$broadcast('MESSAGE_RECEIVED_GLOBAL', data);
                $rootScope.$apply();
            };
        }

    }
})();