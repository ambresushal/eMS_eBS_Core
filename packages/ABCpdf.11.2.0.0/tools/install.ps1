param($installPath, $toolsPath, $package, $project)

$installBuild = join-path $installPath 'build'
$installLib = join-path $installPath (join-path 'lib' 'net40')

function say($x)
{
    if([Environment]::UserInteractive)
    {
        [System.Windows.Forms.MessageBox]::Show($x, "ABCpdf")
    }
	write-host "ABCpdf>$x"
}

if([Environment]::UserInteractive)
{
    add-type -AssemblyName System.Windows.Forms
}

cp (join-path $installLib 'ABCpdf.dll') $installBuild
$pdfSettingsExe = join-path $installBuild 'PDFSettings.exe'

if(![System.IO.File]::Exists($pdfSettingsExe))
{
	say "Error: Unable to insert trial license because $pdfSettingsExe is missing. See ABCpdfQuickStart.txt for details and explanation."
}
elseif([System.Environment]::UserInteractive)
{
    & $pdfSettingsExe "/Register"
}
else
{
    say "Run $pdfSettingsExe to check your license information. See ABCpdfQuickStart.txt for details and explanation."
}

write-host "ABCpdf>Thank you for installing!"
write-host "ABCpdf>See ABCpdfQuickStart.txt for simple test code."
write-host "ABCpdf>See ABCpdf.chm for full documentation."