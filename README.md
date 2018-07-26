# CupheadTAS
Simple TAS tool for the game Cuphead

## TASing the game


Input             | Action
:----------------:|-----------------------:
Left Bumper       | Go back to normal Speed
Right Bumper      | Advance one Frame
Left Stick press  | Enter Recording mode
Right Stick press | Enter Playback mode

 - The control scheme used is the default one.
 - If you enter recording mode whilst in playback mode, your recording will be appended to the `.tas` file that has been loaded

## Input file
If you want to manually edit the file:
<table>
<tr><td>

 - `Number[,char][,char][...]`
    - This sends the corresponding inputs for `Number` frames
    - `X` will read the next character as an analog stick angle (`0`-`360`)
    -  Angle for down is `0` and its increasing clockwise (left -> `90`)
 - `Read,InputFile.tas[,startLine[,stopLine]]`
    - Reads `InputFile.tas` and adds its contents to the Input queue
    - This Version supports relative paths for each file
    - `startLine`, when specified, will start reading at the specified line
    - `stopLine` needs `startLine`. It stops after reaching the specified line
      - The number is inclusive (`stopLine=2` will read line 2)
      - Both numbers start with first line as `1`
 - `***Number`
    - This will set fps to `Number`, `1` is framestep
    - Will probably easily desync, use with caution
 - `# This is a comment`
    - Comment message can be anything
</td><td>

char|In-game action
:--:|-------------:
`<` | Left
`>` | Right
`^` | Up
`v` | Down
`J` | Jump
`D` | Dash
`C` | Change Weapon
`S` | Start
`A` | Attack
`E` | Ex
`L` | Lock
`X` | Analog Stick
</td></tr>
<tr><td>
Example File:

```tas
#!Cuphead any%
#!:Overworld.tas

# Start new Game
   1,J
# Wait for intro to be skippable
  45
# Skip intro
   1,S
Read,Overworld.tas,2,70

*** 4
# End of TAS
```
</td><td>
Best practices:

```tas
File header
Explain what TAS is doing
#!Cuphead any%
List eventual dependencies
#!:Overworld.tas
```

```
Explain what you are doing
# Start new Game
```

```
v Keep padding of 4 spaces
   1,J
```
</td></tr>
</table>
