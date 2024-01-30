# TS4 Default Replacement Fixer - 2020-10-12 v3.0.0.0, fixes default eyes, eyebrows, makeup, and skins broken by patches  v1.0.0.4

## Additional Information
* This is a fork from MTS. 
* Original description by [CmarNYC](https://modthesims.info/member.php?u=3216596) who retired on earth and is now supporting the TS4 development in heaven.

### Requirements:
* s4pe

## Description

This is a very simple tool to fix default replacement eyes, eyebrows, makeup, and skins that were broken by the June 3 and Oct. 7 patches. Version 3.0 has been tested with human and alien eyes, brows, and skins.

To use, extract TS4DefaultsFixer_3_0_0_0.exe and DefaultCASPs.package from the zip to the same location and run TS4DefaultsFixer_3_0_0_0.exe. Select your default replacement package. When the tool is finished save as a new package with a new name. (Do not save over the original. In V3 it will rename the package if you try to save over the original.) Replace the original package with the new one in your Mods folder.

Now supports texture-only default replacements, both as fixed texture-only defaults or with CASPs. Will optionally remove shine if you choose the 'with CASPs' option. Texture-only will not show the CC wrench icon.

Mac users: This site has a collection of fixed eye replacements by creators who are no longer active. https://maxismatchccworld.tumblr.co...fixed-for-patch

Notes:

- I don't recommend using this on merged packages. Non-default items could be broken or become unusable in S4S or CAS Tools.
- Speaking of, the textures in texture-only packages fixed by this tool will be unreadable by any of the standard tools. Be sure to keep the original if you need to work with the package.
- Eye Shine Removers conflict with eye packages fixed by this tool with CASPs since they both replace the default eye color CASPs. You cannot use both. Shine removers should work with texture-only packages fixed with this tool.
- This is very quick and dirty, so please consider it to be beta-ish and report problems.
- Please save as a new package and keep the original just in case. Do not save with the same name as the original package - You'll get an error and it could ruin the original package. As of V3.0 this tool will rename the package if you try to save with the original name.
- Windows only, sorry.

Technical:

With the new patch the EA eye and makeup RLE2 textures are now empty and there's another resource (type 0x2BC04EDF, named LRLE) with the same instance number that afaik is a new type. CASPs that link to textures seem to be looking for the new resource type first and using an RLE2 only if it doesn't find one. Default replacements that link to the EA texture TGIs are overridden by the 0x2BC04EDF resource with the same instance. The fix is to link to a texture with a new, custom TGI. That's my guess, anyway, I could easily be wrong.

Thanks to Dustvy and Midnitetech for the info that RLE2 textures can be given the LRLE type and the game will read them correctly and use them over EA LRLE textures, so we can have texture-only replacements again, until EA breaks them again anyway.

Additional Credits:
Uses s4pi for package handling, source found here: https://github.com/s4ptacle/Sims4Tools/tree/develop

Updates:

Version 3.0, 10/12/2020:
- Updated with the ability to make texture-only packages.
- Now supports default skins.
- Optionally fix packages with CASPs (has CC wrench, conflicts with shine removers), or texture-only (no CC wrench, does not conflict).

Version 2.2, 8/4/2020:
- Updated for new version of CASP in knitting patch. Needed for default replacements created after the patch.

Version 2.0, 6/7/20:
- Name change!
- Added support for eyebrows, lipstick, eyeliner, blush, and eyeshadow.
- Added support for texture-only replacements by copying the corresponding EA CASP.
- Added optional shine removal.

Version 1.2, 6/5/20: Turned on compression so converted packages should now not be bigger than the original.

Version 1.1, 6/5/20: That didn't take long! Now supports secondary eye colors. 