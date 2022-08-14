$file1 = 'settings-testing.yml'
$file2 = 'settings-testing2.yml'
$regBranch = '^branching.*$'
$regItem = '^item.*$'

(Get-Content $file1) -replace $regBranch, ('branchingFactor: ' + $Args[0]) | Set-Content $file1
(Get-Content $file1) -replace $regItem, ('itemSize: ' + $Args[1]) | Set-Content $file1
(Get-Content $file2) -replace $regBranch, ('branchingFactor: ' + $Args[0]) | Set-Content $file2
(Get-Content $file2) -replace $regItem, ('itemSize: ' + $Args[1]) | Set-Content $file2