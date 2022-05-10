I. Saving and loading maps within the MapGen scene
II. Loading maps while the game is running

--------------------------------------------------------------------

I. Instructions for saving and loading maps within the MapGen Scene:

SETUP:
- Open the scene "MapGen"
- Expand the GameManager object so that the GameDB object is visible
- Ensure the GameDB object's Tile Data List is up-to-date with your project's Tile Data
	(NOTE: The saving/loading algorithm generates grids based on the order of tiles in this list. Removing, replacing, or reordering tiles in this list may mess up your old grids.)


SAVING:
- Create your grid normally using your Tile Palette
- Open up the MapGenerator object in the Inspector
- Specify your desired output file name in the field "Output File Name"
- RIGHT CLICK on the component header which reads "Map Generator (Script)"
- The second to last item on this drop-down is "Convert Grid to Array". Click on this.
- Congratulations! A new output file should have appeared in the folder specified by the "Base Path" field (default is "Assets/Resources/Maps/").


LOADING:
- Open up the MapGenerator object in the Inspector
- Drag the grid input file into the field "Input Txt"
- RIGHT CLICK on the component header which reads "Map Generator (Script)"
- At the bottom of this drop-down you will see "Load Grid from Array". Click on this.
- The tiles which correspond to your input text should appear in your scene!

--------------------------------------------------------------------

II. Instructions for loading maps while the game is running
- Obtain your current level's TextAsset (.txt) file. How you manage your level files is up to you.
- Call TilemapManager.instance.LoadGridFromArray(inputFile.txt). Your map should now be loaded!