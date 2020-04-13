
public class Map
{
    private int width { get; set; }
    private int height { get; set; }

    private WorldTile[,] tiles { get; set; }

    public bool mapGenerated;

    public Map(int width, int height)
    {
        this.height = height;
        this.width = width;
        this.tiles = new WorldTile[width,height];
    }

    public WorldTile[,] getTiles() {
        return this.tiles;
    }
}
