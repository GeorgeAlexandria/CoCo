[![License](https://img.shields.io/apm/l/vim-mode.svg)](LICENSE.txt) [![VS14](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS14-v2.0.0-green.svg)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) [![VS15](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS15-v2.0.0-green.svg)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226)

# CoCo
A Visual Studio 2015 (VS14) and 2017 (VS15) extension that uses Roslyn API's for analyzing C# source code
and colorize appropriate syntax nodes to different colors. It makes easily recognizabling the supported elements. 

Extension supports following elements:
* Namespaces and aliases for them
* Local and range variables
* Parameters
* Instance methods and constructors
* Static and extension methods
* Local functions (supports only for VS15)
* Events
* Properties
* Instance and enum fields
* Labels

Use **CoCo/Classifications** options in the Visual Studio options to change colors for items or use **CoCo/Presets** to manage (create, delete) presets, 
<br/>then apply them and accept your changes in the Visual Studio option page (just click OK button).

# Examples

Extension doesn't apply the ***default*** preset after installation. <br/>So you need to set colors manually or apply existing presets as pointed out above if you want to see how it works.

You can look at the screenshot where code was classified after applying the **CoCo dark theme** preset:

<details>
<summary>Click to expand screenshot</summary>

![](https://georgealeksandria.gallerycdn.vsassets.io/extensions/georgealeksandria/coco-19226/1.0/1504035613003/277591/1/DarkExample.PNG)

</details>


And you can look at the similar screenshot where code was classified after applying another preset - **CoCo light|blue theme**:

<details>
<summary>Click to expand screenshot</summary>

![](https://georgealeksandria.gallerycdn.vsassets.io/extensions/georgealeksandria/coco-19226/1.0/1504035613003/277592/1/LightExample.PNG)

</details>

## Contributing
For more information on contributing to the project, please see [CONTRIBUTING.md](https://github.com/GeorgeAlexandria/CoCo/blob/dev/CONTRIBUTING.md)
