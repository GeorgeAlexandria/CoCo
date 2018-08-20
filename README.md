[![License](https://img.shields.io/apm/l/vim-mode.svg)](LICENSE.txt) [![VS14](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS14-v2.0.0-green.svg)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) [![VS15](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS15-v2.0.0-green.svg)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226)

# CoCo
A Visual Studio 2015 (VS14) and 2017 (VS15) extension that uses Roslyn API's for analyzing C# source code
to colorize appropriate syntax nodes to different colors and decorate them to different styles. It makes easily recognizabling the supported elements. 

Extension supports the following elements:
* Namespaces and aliases for them
* Local and range variables
* Parameters
* Instance methods 
* Constructors and destructors
* Static and extension methods
* Local functions (supports only for VS15)
* Events
* Properties
* Instance, enum and constant fields
* Labels

And supports the following decorations:
* Changing foreground and background
* Using bold and italic font
* Changing font rendering size

# Examples

The following screenshots show a different applying decorations and colors to the code

![](https://user-images.githubusercontent.com/13402478/44366917-e82cb200-a4d6-11e8-8a40-d58418bcd08e.png)

![](https://user-images.githubusercontent.com/13402478/44366921-eb27a280-a4d6-11e8-9ba5-c8f7bce5f1ad.png)


# How to use extension
Use **CoCo/Classifications** option page in the Visual Studio options to change colors of items or to apply decorations

![](https://user-images.githubusercontent.com/13402478/44366474-7b64e800-a4d5-11e8-8b82-f844050e5707.png)

You need to click **OK** button to confirm your changes.

Also you can use **CoCo/Presets** option page to save your current settings as preset and to apply or delete existing presets

![](https://user-images.githubusercontent.com/13402478/44367297-0646e200-a4d8-11e8-9b7e-0e6f8c1ce554.png)

You also need to click **OK** button to confirm applying, creating or deleting preset.

If you will not click the **OK** button extension will use the previous (or default) settings for both of pages.

Extension doesn't apply the ***default*** preset after installation. <br/>So you need to set colors and decorations manually or apply existing presets as pointed out above if you want to see how extension works.

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
