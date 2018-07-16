# CupheadTAS
Simple TAS tool for the game Cuphead

## Input file
A line consists of:

<table>
<tr><td>

 - `Number[,char][,char][...]`
    - This sends the corresponding inputs for `Number` frames
    - `X` will read the next character as an analog stick angle (`0`-`360`)
    -  Angle for down is `0` and its increasing clockwise (left -> `90`)
 - `Read,InputFile.tas[,startLine[,linesToRead]]`
    - Reads `InputFile.tas` and adds its contents to the Input queue
    - This Version supports relative paths for each file
    - `startLine`, when specified, will start reading at the specified line
    - `linesToRead` needs `startLine`. It stops after reaching the specified line
      - The number is inclusive (`linesToRead=2` will read line 2)
      - Both numbers start with first line as `1`
 - `***Number`
    - This will somehow modify the speed in relation to `Number`
    - Will probably easily desync, use with caution
 - `# This is a comment`
    - Comment message can be anything

</td><td>

char|In-game action|
:-:|-:
`<`|Left
`>`|Right
`^`|Up
`v`|Down
`J`|Jump
`D`|Dash
`C`|Change Weapon
`S`|Start
`A`|Attack
`E`|Ex
`L`|Lock
`X`|Analog Stick

</td></tr> </table>
