angular.module("TestApp", [])
    .controller("diskController", function ($scope, $http) {
        $http.get("/api/disk").then(function (response) {
            $scope.items = response.data;
            $scope.curPath = "";
            $scope.sizes = [0,0,0];
        },
        function (response) {
            alert("Can't read disk info");
        });

        $scope.getDirectory = function (dir) {
            if (".." === dir) { // go to one level up
                var arr = $scope.curPath.split('\\');
                arr.pop();
                $scope.curPath = arr.join('\\');
            }
            else {
                if (0 == $scope.curPath.length) {   // don't prepend extra slash to disk names
                    $scope.curPath = dir;
                }
                else {
                    $scope.curPath = $scope.curPath + "\\" + dir;
                }
            }

            // check whether we can't go up in folder tree
            if ($scope.curPath.includes('\\')) {
                $http.get("/api/disk/?directory=" + $scope.curPath).then(function (response) {
                    $scope.items = [];  // clear folders
                    $scope.folderfiles = []; // clear files
                    $scope.items[0] = "..";
                    $scope.sizes = response.data.sizes;
                    console.log(response.data.sizes);

                    // iterate over folders
                    for (i = 0; i < response.data.folders.length; i++) {
                        var item = response.data.folders[i].split('\\').pop();
                        $scope.items[i + 1] = item;
                    };

                    // iterate over files
                    for (i = 0; i < response.data.files.length; i++) {
                        var file = response.data.files[i].split('\\').pop();
                        console.log(file);
                        $scope.folderfiles[i] = file;
                    };
                },
                function (response) {
                    alert("Folder doesn't exist or some other error happened");

                    // cleanup path
                    var arr = $scope.curPath.split('\\');
                    arr.pop();
                    $scope.curPath = arr.join('\\');

                    // check for top disk case
                    if (!$scope.curPath.includes('\\'))
                    {
                        $scope.curPath = "";
                    }

                    console.log($scope.curPath);
                });
            }
            else // need to show logical disks
            {
                $scope.items = [];  // clear folders
                $scope.folderfiles = []; // clear files
                $http.get("/api/disk/").then(function (response) {
                    $scope.items = response.data;
                    $scope.curPath = "";
                    $scope.sizes = [0, 0, 0];
                },
                function (response) {
                    alert("Can't read disk info");

                    $scope.curPath = "";
                });
            }
        }; // getDirectory
    });