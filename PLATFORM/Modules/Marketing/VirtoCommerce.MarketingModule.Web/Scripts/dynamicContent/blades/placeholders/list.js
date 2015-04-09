﻿angular.module('virtoCommerce.marketingModule')
.controller('placeholdersDynamicContentListController', ['$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
	var blade = $scope.blade;
	blade.choosenFolder = undefined;
	blade.currentEntity = undefined;
	blade.currentEntities = [];

	$scope.selectedNodeId = null;

	function initializeBlade() {
		blade.isLoading = false;
	};

	blade.addNew = function () {
		blade.closeChildrenBlades();

		var newBlade = {
			id: 'listItemChild',
			title: 'New placeholders element',
			subtitle: 'Add new placeholders element',
			choosenFolder: blade.choosenFolder,
			controller: 'addPlaceholderElementController',
			template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/add.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, $scope.blade);
	}

	blade.addNewFolder = function (data) {
		blade.closeChildrenBlades();

		var newBlade = {
			id: 'listItemChild',
			title: 'New placeholders element',
			subtitle: 'Add new placeholders element',
			entity: data,
			isNew: true,
			controller: 'addFolderPlaceholderController',
			template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/folder-details.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, $scope.blade);
	}

	blade.addNewPlaceholder = function (data) {
		blade.closeChildrenBlades();

		var newBlade = {
			id: 'listItemChild',
			title: 'New placeholders element',
			subtitle: 'Add new placeholders element',
			entity: data,
			isNew: true,
			controller: 'addPlaceholderController',
			template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/placeholder-details.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, $scope.blade);
	}

	blade.closeChildrenBlades = function() {
		angular.forEach(blade.childrenBlades.slice(), function (child) {
			bladeNavigationService.closeBlade(child);
		});
	}

	blade.folderClick = function (data) {
		if (angular.isUndefined(blade.currentEntity) || angular.equals(blade.currentEntity, data)) {
			blade.choosenFolder = data.id;
			blade.currentEntity = data;
		}
		else {
			blade.choosenFolder = undefined;
			blade.currentEntity = undefined;
		}
	}

	blade.checkFolder = function (data) {
		return angular.equals(data, blade.currentEntity);
	}

	$scope.bladeToolbarCommands = [
        {
        	name: "Refresh", icon: 'fa fa-refresh',
        	executeMethod: function () {
        		$scope.blade.refresh();
        	},
        	canExecuteMethod: function () {
        		return true;
        	}
        },
        {
        	name: "Add", icon: 'fa fa-plus',
        	executeMethod: function () {
        		blade.addNew();
        	},
        	canExecuteMethod: function () {
        		return true;
        	}
        },
	];

	$scope.bladeHeadIco = 'fa fa-flag';

	initializeBlade();
}]);
