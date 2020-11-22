using Microsoft.VisualBasic.FileIO;
using MoneyveoDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoneyveoDemo.Controllers
{
    public class HomeController : Controller
    {
        public Random rnd { get; set; } = new Random();

        public ActionResult Index()
        {
             var n = new Random().Next(2, 20);
            var matrix = new int[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = rnd.Next(0, 100);

            return View(new MatrixModel { matrix = matrix });
        }

        [HttpPost]
        public ActionResult ImportCsv(HttpPostedFileBase file)
        {
            using (TextFieldParser parser = new TextFieldParser(file.InputStream))
            {
                int j = 0;
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                var first = parser.ReadFields();
                var matrix = new int[first.Length, first.Length];
                for (int i = 0; i < first.Length; i++)
                    int.TryParse(first[i], out matrix[j, i]);
                ++j;

                try
                {
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        for (int i = 0; i < fields.Length; i++)
                            int.TryParse(fields[i], out matrix[j, i]);
                        j++;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine("Invalid File");
                }
                return View("Index", new MatrixModel { matrix = matrix});
            }
        }

        [HttpPost]
        public ActionResult Rotate(MatrixModel2 _matrix) // Model binding doesn't work for unknown reason, haven't time to find a root, not a big deal, I will make it myself. int[,] maps to int[] lolx
        {
            StreamReader reader = new StreamReader(Request.InputStream);
            string bodyRaw = reader.ReadToEnd();
            var body = JsonConvert.DeserializeObject<MatrixModel2>(bodyRaw); // getting array that was recieved from client
            int mLength = (int)Math.Sqrt(body.matrix.Length); 

            var matrix = new int[mLength, mLength]; // create instance for rotated matrix


            for (int i=0; i<mLength ; i++)       // making rotate algo with 1 demencion array
                for (int j = 0, o = mLength-1; j < mLength && o >= 0 ; j++, o--)
                    matrix[o, i] = body.matrix[i * mLength + j];


            return PartialView("Index", new MatrixModel { matrix = matrix }); 
        }
    }
}