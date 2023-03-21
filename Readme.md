# Adsk License Modifier

- [ADSKLincensingModify](#adsklincensingmodify)
- [Predecessor](#predecessor)

> Modifies License Information for Autodesk Products 2020+ . This method will not work for older autodesk versions.

If you ever had the challenge to switch the licensing typ of a Autodesk product that does no longer start, then you know exactly why this tool saves a lot of time.
As a CAD-Admin you might have to do this multiple times per week.

For Autodesk 2020 product this means, you have to use AdskLicensingInstHelper.exe and some parameters. This takes time and knowledge.

Adsk License Modifier is a GUI for this exe and makes this task easy.

> Note: If you want to check out the manual way by autodesk, feel free to use this link: [Autodesk Knowledge](https://knowledge.autodesk.com/search-result/caas/sfdcarticles/sfdcarticles/How-to-change-or-reset-licensing-on-your-Autodesk-software.html)

## Predecessor of ADSKLincensingModify

This tool is the predecessor of the powershell version found in this [repository](https://github.com/TWiesendanger/ADSKLincensingModify).
This tool should have all the same functions but with a much more modern gui framework (winui3) and you can directly download it from microsoft store.

## Start

To get started you need to make sure that the `AdskLicensingInstHelper.exe` is installed. This will be the case if you installed any autodesk product. It is possible to generate command but you wont be able to run them. If you copy them to another client, then this is completly fine and should work. If the check fails some gui elements are not activated.

You can check the path for yourself.

`C:\Program Files (x86)\Common Files\Autodesk Shared\AdskLicensing\Current\helper`

> Note: This exe isn't delivered by this tool. You get it by installing any Autodesk 2020 product. It also get's updated from time to time, which makes it difficult to ship.

## Settings

### Theme

At the moment there is only dark theme and there are no plans to change this in the future.

### Print List command

This creates a json file an tries to open it. If it fails make sure you have a tool to open such files. It show alot of infos about every licensed product on the machine this command was used.

Make sure to check the Autodesk Knowledge article:
[Autodesk Knowledge](https://knowledge.autodesk.com/support/autocad/troubleshooting/caas/sfdcarticles/sfdcarticles/Use-Installer-Helper.html)

| value |      license method      |
| ----- | :----------------------: |
| 0     | Unknown licensing method |
| 1     |    Network licensing     |
| 2     |   Standalone licensing   |
| 3     |          (MSSA)          |
| 4     |      User Licensing      |

| value |     server type     |
| ----- | :-----------------: |
| 0     | Unknown server type |
| 1     |    Single server    |
| 2     |  Redundant servers  |
| 3     | Distributed servers |

You can copy the command that is run when the button to the right is clicked.

### Quick Links

There are three links that help to navigate to some folders that are often used for fixing license problems. By clicking the buttons a windows explorer is started.










## License

MIT License

Copyright (c) 2023 Tobias Wiesendanger

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
