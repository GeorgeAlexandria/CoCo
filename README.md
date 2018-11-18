[![License](https://img.shields.io/apm/l/vim-mode.svg?style=flat-square)](LICENSE.txt) [![VS14](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS14-v2.5.0-orange.svg?style=flat-square)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) [![VS15](https://img.shields.io/badge/Visual%20Studio%20Marketplace%20%7C%20VS15-v2.5.0-orange.svg?style=flat-square)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226)

# CoCo
A Visual Studio [2015](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) (VS14) and [2017](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226) (VS15) extension that is analyzing C#, VB\.Net source codes to colorize 
appropriate syntax nodes to different colors and decorate them to different styles. It makes easily recognizabling the supported elements. 

## Supported items 

Extension supports the following language elements:

#### CSharp

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
* Class, structure and interface types
* Enum, delegate and type parameter types

#### VisualBasic

* Namespaces and aliases for them
* Local and range variables
* Static local and function variables
* Parameters
* Function and sub methods 
* Shared and extension methods
* Events
* Properties and "WithEvents" fields
* Instance, enum and constant fields
* Class, structure and module types
* Enum and delegate types
* Interface and type parameter types

supports the following decorations:
* Changing foreground and background
* Changing font family
* Changing font styles and stretches
* Using bold font weight
* Changing font rendering size
* Using overline, underline, strikethrough and baseline font

and supports the following analyze options:
* Disable classification a current item
* Disable classification a current item inside a xml doc comments

## Examples

The following screenshots show a different applying decorations and colors to the code:

![](https://user-images.githubusercontent.com/13402478/44617734-03017c80-a871-11e8-86ac-5cc4e0c4d73f.png)

![](https://user-images.githubusercontent.com/13402478/44617735-04cb4000-a871-11e8-9f69-52caf1210996.png)

![](https://user-images.githubusercontent.com/13402478/48673969-722cfb00-eb58-11e8-9896-6caa1188ee1d.png)


## Usage 
Use **CoCo/Classifications** option page in the Visual Studio options to change decorations of items or their analyze options:

![](https://user-images.githubusercontent.com/13402478/48673975-83760780-eb58-11e8-9cce-b0a7eb8f05a7.png)


You can disable classification of the chosen item in the **Analyze settings** sections to allow the other similar extensions or Visual Studio classify this item.

Also you can use **CoCo/Presets** option page to save your current settings as preset and to apply or delete existing presets:

![](https://user-images.githubusercontent.com/13402478/44617733-009f2280-a871-11e8-8619-35aadf25f734.png)

You need to click **OK** button to confirm your changes on the **CoCo/Classifications** and 
to confirm applying, creating or deleting preset on the **CoCo/Presets**. If you will not click the **OK** button 
extension will use the previous (or default) settings for both of pages. Settings are stored under **%localappdata%\CoCo\CoCo.config**.

### After installation
Extension doesn't apply the one of ***default*** presets after installation. It only apply settings from config file if it exists on your system, 
else you need to set colors and decorations manually or apply existing presets as pointed out above if you want to see how the extension works.

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

## Changelog
For more information on project's changelog, please see [ReleaseNotes.md](https://github.com/GeorgeAlexandria/CoCo/blob/dev/ReleaseNotes.md)

## Contributing
For more information on contributing to the project, please see [CONTRIBUTING.md](https://github.com/GeorgeAlexandria/CoCo/blob/dev/CONTRIBUTING.md)

## License

CoCo is licensed under the [MIT license](https://github.com/GeorgeAlexandria/CoCo/blob/dev/LICENSE.txt)