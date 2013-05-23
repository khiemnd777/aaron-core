param($installPath, $toolsPath, $package, $project)

# Read the transformed text from the custom template included in the package
$customGlobalAsax = $project.ProjectItems | where { $_.Name -eq "Global.asax.cs.custom" }
$customGlobalAsax.Open()
$customGlobalAsax.Document.Activate()
$customGlobalAsax.Document.Selection.SelectAll(); 
$replacementGlobalAsax = $customGlobalAsax.Document.Selection.Text;
$customGlobalAsax.Delete()

# Replace the contents of Global.asax.cs
$globalAsax = $project.ProjectItems | ForEach-Object { $_.ProjectItems } | where { $_.Name -eq "Global.asax.cs" }
if($globalAsax) {
    $globalAsax.Open()
    $globalAsax.Document.Activate()
    $globalAsax.Document.Selection.SelectAll()
    $globalAsax.Document.Selection.Insert($replacementGlobalAsax)
    $globalAsax.Document.Selection.StartOfDocument()
    $globalAsax.Document.Close(0)
}

# Remove the folder named 'App_Start'
$DTE.Solution.Projects | ForEach { $_.ProjectItems | ForEach { if ($_.Name -eq "App_Start") { $_.Remove() } } }