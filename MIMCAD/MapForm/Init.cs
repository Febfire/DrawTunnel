using System.Drawing;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap;
using SharpMap.Styles;
using SharpMap.Rendering;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using SharpMap.Rendering.Thematics;
using static SharpMap.Rendering.Decoration.GoogleMapsDisclaimer;
using System;

namespace MapForm
{
    static class Init
    {
        static public string sPath = Environment.GetEnvironmentVariable("ACAD_RES");
        static public string mapPath = sPath + @"\GeoData\";
        public static Map InitializeMapOrig(string path)
        {
            //Initialize a new map of size 'imagesize'
            Map map = new Map();

            //Set up the countries layer
            VectorLayer layCountries = new VectorLayer("Countries");
            //Set the datasource to a shapefile in the App_data folder
            layCountries.DataSource = new ShapeFile(path, true);
            //Set fill-style to green
            layCountries.Style.Fill = new SolidBrush(Color.FromArgb(64, Color.Green));
            //Set the polygons to have a black outline
            layCountries.Style.Outline = Pens.Black;
            layCountries.Style.EnableOutline = true;
            layCountries.SRID = 4326;
            
            //Set up a country label layer
            LabelLayer layLabel = new LabelLayer("Country labels");
            layLabel.DataSource = layCountries.DataSource;
            layLabel.Enabled = true;
            layLabel.LabelColumn = "Name";
            layLabel.Style = new LabelStyle();
            layLabel.Style.ForeColor = Color.White;
            layLabel.Style.Font = new Font(FontFamily.GenericSerif, 12);
            layLabel.Style.BackColor = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
            layLabel.MaxVisible = 90;
           // layLabel.MinVisible = 30;
            layLabel.Style.HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center;
            layLabel.SRID = 4326;
            layLabel.MultipartGeometryBehaviour = LabelLayer.MultipartGeometryBehaviourEnum.Largest;
            layLabel.LabelFilter = LabelCollisionDetection.ThoroughCollisionDetection;
            layLabel.Style.CollisionDetection = true;
            layLabel.LabelPositionDelegate = fdr => fdr.Geometry.InteriorPoint.Coordinate;
            layLabel.PriorityColumn = "POPDENS";
            
            //Add the layers to the map object.
            //The order we add them in are the order they are drawn, so we add the rivers last to put them on top
            //map.BackgroundLayer.Add(AsyncLayerProxyLayer.Create(layCountries));
            map.Layers.Add(layCountries);
            map.Layers.Add(layLabel);
            map.Zoom = 360;
            map.MapScale = 350;


            Matrix mat = new Matrix();
            mat.RotateAt(0, map.WorldToImage(map.Center));
            map.MapTransform = mat;
            return map;
        }
    }
    
}
