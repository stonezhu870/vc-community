﻿angular.module('virtoCommerce.storeModule')
.controller('virtoCommerce.storeModule.storeLanguagesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            itemId: blade.itemId,
            title: blade.title,
            subtitle: 'Manage languages',
            controller: 'virtoCommerce.storeModule.storeLanguagesController',
            template: 'Modules/$(VirtoCommerce.Store)/Scripts/blades/store-languages.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);