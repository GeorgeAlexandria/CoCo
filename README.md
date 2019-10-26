[![License](https://img.shields.io/apm/l/vim-mode.svg?style=flat-square)](LICENSE.txt) [![VS14](https://img.shields.io/badge/Visual%20Studio%202015%20Marketplace-v3.2.2-orange.svg?style=flat-square)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) [![VS15](https://img.shields.io/badge/Visual%20Studio%202017%20Marketplace-v3.2.2-orange.svg?style=flat-square)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226) [![VS16](https://img.shields.io/badge/Visual%20Studio%202019%20Marketplace-v3.2.2-orange.svg?style=flat-square)](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-vs16)

# CoCo
A Visual Studio [2015](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo) (vs14), [2017](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-19226) (vs15) and [2019](https://marketplace.visualstudio.com/items?itemName=GeorgeAleksandria.CoCo-vs16) (vs16) extension that is analyzing source code to colorize and decorate appropriate syntax nodes to different colors and different styles. It makes easily to read and understand sources, to find sought entities and to navigate by code. 

## Features

Currently extension supports analyzing C#, VB\.Net source codes. It's colorize and decorate a many semantic and language elements such as **variables**, **methods**, **types**, **members**, **namespaces** and many others. For more detailed list of supported items, please see [Classifications.md](https://github.com/GeorgeAlexandria/CoCo/blob/dev/Classifications.md).

Extension can apply the following decorations to analyzed elements:
* Changing foreground and background
* Changing font family
* Changing font styles and stretches
* Using bold font weight
* Changing font rendering size
* Using overline, underline, strikethrough and baseline font

supports the following analyze options of elements:
* Disable element classification at all
* Disable element classification in editor text
* Disable element classification inside xml doc comments in editor text
* Disable element classification in quick info tooltip

and can apply the chosen settings to:
* Editor text
* Quick info tooltip

## Examples

The following screenshots show a different applying decorations and colors to the code:

![](https://user-images.githubusercontent.com/13402478/56852109-658aab00-691f-11e9-9a2a-311bb47cac99.png)

![](https://user-images.githubusercontent.com/13402478/56852108-658aab00-691f-11e9-97a2-dda4988235bc.png)

![](https://user-images.githubusercontent.com/13402478/56852110-658aab00-691f-11e9-98c4-32a5b7eb3767.png)

![](https://user-images.githubusercontent.com/13402478/56852111-66234180-691f-11e9-8f9b-093d9b429811.png)

## How to use 
In **CoCo/General** option page of the Visual Studio you can fully enable or disable editor and quick info classification:

![](https://user-images.githubusercontent.com/13402478/56852160-2ad54280-6920-11e9-836e-0051b743394b.png)

In **CoCo/Classifications** you can manage all supported classifications. 

In the ***Classifications*** tab you can set decorations and analyze options for classification item:

![](https://user-images.githubusercontent.com/13402478/56852159-2ad54280-6920-11e9-93f9-bfb74d802188.png)



and in the ***Presets*** tab you can save your current settings from **Classifications** tab as preset and apply or delete existing presets:

![](https://user-images.githubusercontent.com/13402478/56852238-4d1b9000-6921-11e9-91ad-d535e92d326e.png)


You need to click **OK** button to confirm your changes on the all CoCo options pages. If you will not click the **OK** button 
extension will use the previous (or default) settings for all of pages. All settings are stored under **%localappdata%\CoCo\\**.


### Installation
Extension doesn't apply the one of **default presets** after the **first installation** on a system. Therefore, you need to set colors and decorations manually or **apply existing presets** as pointed out above. After **updating extension** to a new version it will use the stored settings files on your system if they exist otherwise you still need to apply some colors and decorations.

### Presets
You can look at the screenshot below where code was colorized and decorate after applying the **CoCo dark theme** preset:

<details>
<summary>Click to expand screenshot</summary>
  
![](https://georgealeksandria.gallerycdn.vsassets.io/extensions/georgealeksandria/coco-19226/1.0/1504035613003/277591/1/DarkExample.PNG)

</details>

And you can look at the similar screenshot where code was colorized and decorate after applying another preset - **CoCo light|blue theme**:

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