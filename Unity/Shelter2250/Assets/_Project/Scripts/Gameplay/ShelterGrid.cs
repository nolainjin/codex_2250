using System;
using System.Collections.Generic;

namespace Shelter2250.Gameplay
{
    public sealed class ShelterGrid
    {
        private readonly List<RoomInstance> rooms;

        public ShelterGrid(List<RoomInstance> rooms)
        {
            this.rooms = rooms ?? throw new ArgumentNullException(nameof(rooms));
        }

        public IReadOnlyList<RoomInstance> Rooms => rooms;

        public PlacementResult ValidatePlacement(RoomInstance candidate, bool allowIsolated)
        {
            if (candidate == null) return PlacementResult.Fail("candidate is null");

            if (candidate.width <= 0 || candidate.height <= 0) return PlacementResult.Fail("invalid size");

            for (var i = 0; i < rooms.Count; i++)
            {
                if (Overlaps(rooms[i], candidate))
                {
                    return PlacementResult.Fail("overlaps existing room");
                }
            }

            if (!allowIsolated && rooms.Count > 0)
            {
                var hasAdjacency = false;
                for (var i = 0; i < rooms.Count; i++)
                {
                    if (IsAdjacent(rooms[i], candidate))
                    {
                        hasAdjacency = true;
                        break;
                    }
                }

                if (!hasAdjacency)
                {
                    return PlacementResult.Fail("must be adjacent to an existing room");
                }
            }

            return PlacementResult.Ok();
        }

        public bool TryAdd(RoomInstance room, bool allowIsolated, out string reason)
        {
            var result = ValidatePlacement(room, allowIsolated);
            if (!result.ok)
            {
                reason = result.reason;
                return false;
            }

            rooms.Add(room);
            reason = null;
            return true;
        }

        private static bool Overlaps(RoomInstance a, RoomInstance b)
        {
            var ax2 = a.x + a.width;
            var ay2 = a.y + a.height;
            var bx2 = b.x + b.width;
            var by2 = b.y + b.height;

            return a.x < bx2 && ax2 > b.x && a.y < by2 && ay2 > b.y;
        }

        private static bool IsAdjacent(RoomInstance a, RoomInstance b)
        {
            // Adjacent if they share an edge (not diagonal)
            var ax1 = a.x;
            var ax2 = a.x + a.width;
            var ay1 = a.y;
            var ay2 = a.y + a.height;

            var bx1 = b.x;
            var bx2 = b.x + b.width;
            var by1 = b.y;
            var by2 = b.y + b.height;

            var verticalTouch = (ax2 == bx1 || bx2 == ax1) && RangesOverlap(ay1, ay2, by1, by2);
            var horizontalTouch = (ay2 == by1 || by2 == ay1) && RangesOverlap(ax1, ax2, bx1, bx2);

            return verticalTouch || horizontalTouch;
        }

        private static bool RangesOverlap(int a1, int a2, int b1, int b2)
        {
            return a1 < b2 && a2 > b1;
        }

        public readonly struct PlacementResult
        {
            public readonly bool ok;
            public readonly string reason;

            private PlacementResult(bool ok, string reason)
            {
                this.ok = ok;
                this.reason = reason;
            }

            public static PlacementResult Ok() => new PlacementResult(true, null);
            public static PlacementResult Fail(string reason) => new PlacementResult(false, reason);
        }
    }
}

