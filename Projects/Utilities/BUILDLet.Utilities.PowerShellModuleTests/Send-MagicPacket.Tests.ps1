<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

 Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
################################################################################>

#
# This is a PowerShell Unit Test file.
# You need a unit test framework such as Pester to run PowerShell Unit tests. 
# You can download Pester from http://go.microsoft.com/fwlink/?LinkID=534084
#

# Initialize
. ($PSCommandPath | Split-Path -Parent | Join-Path -ChildPath 'Initialize.ps1') > $null


# Local Data Directory
$Script:dirpath_Local = '..\..\..\..\..\Local'

# MAC Address Text File
$Script:filepath_MacAddress = $Script:dirpath_Local | Join-Path -ChildPath 'MacAddress.txt'

# Magic Packet
$Script:packet


Describe "Send-MagicPacket" {

	Context "Magic Packet" {

		$mac_address = Get-Content -Path $Script:filepath_MacAddress

		It "(MAC Address:$mac_address) is sent." { $Script:packet = Send-MagicPacket -MacAddress $mac_address -Count 3 -PassThru }

		for ($i = 0; $i -lt 17; $i++) {

			$test_text = [string]::Empty
			for ($j = 0; $j -lt 6; $j++) { $test_text += [string]::Format('{0:X2}', $Script:packet[(6 * $i) + $j]) }

			if ($i -eq 0) {
				It ("Consists of 0x$test_text. (" + [string]::Format("{0:D2}", $i) + ")") { $test_text | Should Be 'FFFFFFFFFFFF' }
			}
			else {
				It ("Consists of 0x$test_text. (" + [string]::Format("{0:D2}", $i) + ")") { $test_text | Should Be ([string]$mac_address).Replace(':','') }
			}
		}
	}
}
