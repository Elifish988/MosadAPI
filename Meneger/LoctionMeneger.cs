using MosadApi.Models;
using System;

namespace MosadApi.Helper

{
    public static class LoctionMeneger
    {
        public static Location ChangeLocation(Location location, Direction direction)
        {

            switch (direction)
            {

                case Direction.e:
                    location.x = Plus(location.x);
                    break;

                case Direction.n:
                    location.y = Less(location.y);
                    break;

                case Direction.w:
                    location.x = Less(location.x);
                    break;

                case Direction.s:
                    location.y = Plus(location.y);
                    break;

                case Direction.ne:
                    location.y = Less(location.y);
                    location.x = Plus(location.x);
                    break;

                case Direction.se:
                    location.y = Plus(location.y);
                    location.x = Plus(location.x);
                    break;

                case Direction.sw:
                    location.y = Plus(location.y);
                    location.x = Less(location.x);
                    break;

                case Direction.nw:
                    location.y = Less(location.y);
                    location.x = Less(location.x);
                    break;
            }
            return location;

        }

        public static bool InRange(int num)
        {
            if (num < 1 && num > 1000)
            {
                return false;
            }
            return true;

        }

        public static int Plus(int num)
        {
            int tmp = num + 1;
            if (InRange(tmp))
            {
                return tmp;
            }
            return num;
        }

        public static int Less(int num)
        {
            int tmp = num - 1;
            if (InRange(tmp))
            {
                return tmp;
            }
            return num;
        }
    }
}
