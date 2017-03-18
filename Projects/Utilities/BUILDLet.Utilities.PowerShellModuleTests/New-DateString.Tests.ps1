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


Describe "New-DateString" {

	# Preparation
	[string]$sample_date = "2016/8/9 13:00:00"


	# Default
	Context "Date String without any Parameters (Default: Today)" {
		$expected_pattern = "\d{4}”N\d{1,2}ŒŽ\d{1,2}“ú"
		It "is Like '$expected_pattern'." { New-DateString | Should Match $expected_pattern }
	}


	# 'Date' Parameter
	Context "Date String of '$sample_date' ('Date' Parameter Test)" {
		$expected = "2016”N8ŒŽ9“ú"
		It "is '$expected'." { New-DateString -Date $sample_date | Should Be $expected }
	}


	# 'Format' Parameter
	Context "Date String of '$sample_date' with Some 'Format' Parameter" {

		$parameters = @{
			'F' = "2016”N8ŒŽ9“ú 13:00:00"
			'g' = "2016/08/09 13:00"
			"yyyy-MM-dd (dddd) HH:mm:ss" = "2016-08-09 (‰Î—j“ú) 13:00:00"
		}

		$parameters.Keys | % {
			$format = $_
			$expected = $parameters[$_]

			It "is '$expected'. (Format='$format')" {
				New-DateString -Date $sample_date -Format $format | Should Be $expected
			}
		}
	}


	# 'LCID' Parameter
	Context "Date String of Today (Default) with Some 'LCID' parameter" {

		$parameters = @(
			"en"
			"en-US"
			"ja"
			"ja-JP"
			"fr"
			"it"
			"de"
			"es"
			"zh-CN"
			"zh-TW"
			"ru"
			"pl"
			"tr"
			"ar"
			"ar-AE"
			"ar-SA"
		) | % {
			$lcid = $_
			$expected = (Get-Date).ToString('D', (New-Object System.Globalization.CultureInfo($lcid)))

			It "Is '$expected'. (LCID='$lcid')" { New-DateString -LCID $lcid | Should Be $expected }
		}
	}

	# Complex
	Context "Date String for Complex Test" {

		# Format="yyyy-MM-dd (dddd) HH:mm:ss", LCID="en-US"
		$format = "yyyy-MM-dd (dddd) HH:mm:ss"
		$lcid = "en-US"
		$expected = "2016-08-09 (Tuesday) 13:00:00"
		It "Is '$expected'. (Format='$format', LCID='$lcid')" {
			New-DateString -Date $sample_date -LCID $lcid -Format $format | Should Be $expected
		}

		# Date=Sample Date, LCID="en-US"
		$date = $sample_date
		$lcid = "en-US"
		$expected = "Tuesday, August 9, 2016"
		It "Is '$expected'. (Date='$date', LCID='$lcid')" {
			New-DateString -Date $date -LCID $lcid | Should Be $expected
		}
	}
}
