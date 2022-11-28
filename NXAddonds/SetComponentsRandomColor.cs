using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using System.Collections.Generic;

namespace SetRandomColor
{
    class SetComponentsRandomColor
    {
        public static void Main(string[] args)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;

                // a list of priority colors that are assigned to the components of the assembly
                // YOU CAN CHANGE THIS LIST
                int[] colorValues = { 202, 192, 156, 131, 107, 137, 177, 168,
                                184, 84, 95, 66, 102, 140, 135, 194, 191,
                                183, 124, 90, 65, 165, 88,
                                57, 121, 80};
                int colorsAmount = colorValues.Length;

                List<string> partNameList = new List<string>();

                List<Component> components = new List<Component>();
                components.AddRange(workPart.ComponentAssembly.RootComponent.GetChildren());
                foreach (Component item in components)
                {
                    try
                    {
                        string partName = GetFullNamePart(item);
                        int colorIndex = partNameList.IndexOf(partName);
                        if (colorIndex == -1)
                        {
                            partNameList.Add(partName);
                            colorIndex = partNameList.Count - 1;
                        }

                        DisplayModification displayModification1 = theSession.DisplayManager.NewDisplayModification();
                        displayModification1.ApplyToAllFaces = false;
                        displayModification1.ApplyToOwningParts = true;

                        if (colorsAmount > colorIndex)
                            displayModification1.NewColor = colorValues[colorIndex];
                        else
                            displayModification1.NewColor = Clamp(colorIndex, colorsAmount, 216);

                        DisplayableObject[] objects1 = new DisplayableObject[1];
                        objects1[0] = item;
                        displayModification1.Apply(objects1);

                        displayModification1.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }


        public static string GetFullNamePart(Component theComponent)
        {
            string part_name;
            string refset_name;
            string instance_name;
            double[] origin = new double[3];
            double[] csys_matrix = new double[9];
            double[,] transform = new double[4, 4];

            UFSession.GetUFSession().Assem.AskComponentData(theComponent.Tag,
                    out part_name, out refset_name, out instance_name,
                    origin, csys_matrix, transform);

            return part_name;
        }

        public static int GetUnloadOption(string dummy) { return (int)Session.LibraryUnloadOption.Immediately; }
    }
}
