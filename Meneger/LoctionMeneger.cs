using MosadApi.Models;
using System;

namespace MosadApi.Helper

{
    public static class LoctionMeneger
    {

        public static Location ChangeLocation(Location location, string direction)
        {

            switch (direction)
            {

                case "e":
                    location.x = Plus(location.x);
                    break;

                case "n":
                    location.y = Less(location.y);
                    break;

                case "w":
                    location.x = Less(location.x);
                    break;

                case "s":
                    location.y = Plus(location.y);
                    break;

                case "ne":
                    location.y = Less(location.y);
                    location.x = Plus(location.x);
                    break;

                case "se":
                    location.y = Plus(location.y);
                    location.x = Plus(location.x);
                    break;

                case "sw":
                    location.y = Plus(location.y);
                    location.x = Less(location.x);
                    break;

                case "nw":
                    location.y = Less(location.y);
                    location.x = Less(location.x);
                    break;
            }
            return location;

        }


        public static int Plus(int num)
        {
            int tmp = num + 1;
            if (tmp <= 1000)
            {
                return tmp;
            }
            return num;
        }

        public static int Less(int num)
        {
            int tmp = num - 1;
            if (tmp > 0)
            {
                return tmp;
            }
            return num;
        }
    }
}
