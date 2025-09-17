using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VinnyLibConverterCommon.GeometryUtils
{
    /// <summary>
    /// Powereb by Deepseek
    /// </summary>
    public class DelaunayTriangulation
    {
        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            public double Z { get; set; }

            public double[] GetXYZ()
            {
                return new double[] { X, Y, Z };
            }

            public Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }


            public double DistanceSquared(Point other)
            {
                double dx = X - other.X;
                double dy = Y - other.Y;
                return dx * dx + dy * dy;
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }

        public class Triangle
        {
            public Point A { get; set; }
            public Point B { get; set; }
            public Point C { get; set; }

            public Triangle(Point a, Point b, Point c)
            {
                A = a;
                B = b;
                C = c;
            }

            public bool ContainsPoint(Point point)
            {
                return A == point || B == point || C == point;
            }

            public bool ContainsEdge(Point p1, Point p2)
            {
                return (A == p1 || B == p1 || C == p1) &&
                       (A == p2 || B == p2 || C == p2);
            }

            public override string ToString()
            {
                return $"[{A}, {B}, {C}]";
            }

            public override bool Equals(object obj)
            {
                if (obj is Triangle other)
                {
                    var points1 = new List<Point> { A, B, C }.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
                    var points2 = new List<Point> { other.A, other.B, other.C }.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

                    return points1[0] == points2[0] && points1[1] == points2[1] && points1[2] == points2[2];
                }
                return false;
            }

            public override int GetHashCode()
            {
                return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
            }
        }

        public static List<Triangle> Triangulate(List<Point> points)
        {
            if (points == null || points.Count < 3)
                throw new ArgumentException("At least 3 points are required for triangulation");

            // Create a super triangle that contains all points
            var superTriangle = CreateSuperTriangle(points);
            var triangles = new List<Triangle> { superTriangle };

            // Add points one by one
            foreach (var point in points)
            {
                var badTriangles = new List<Triangle>();

                // Find all triangles that are no longer valid due to the new point
                foreach (var triangle in triangles)
                {
                    if (IsPointInCircumcircle(triangle, point))
                    {
                        badTriangles.Add(triangle);
                    }
                }

                var polygon = new List<(Point, Point)>();

                // Find the boundary of the polygonal hole
                foreach (var triangle in badTriangles)
                {
                    var edges = new List<(Point, Point)>
                {
                    (triangle.A, triangle.B),
                    (triangle.B, triangle.C),
                    (triangle.C, triangle.A)
                };

                    foreach (var edge in edges)
                    {
                        bool shared = false;
                        foreach (var otherTriangle in badTriangles)
                        {
                            if (triangle != otherTriangle && otherTriangle.ContainsEdge(edge.Item1, edge.Item2))
                            {
                                shared = true;
                                break;
                            }
                        }

                        if (!shared)
                        {
                            polygon.Add(edge);
                        }
                    }
                }

                // Remove bad triangles
                triangles.RemoveAll(t => badTriangles.Contains(t));

                // Create new triangles from the point to each edge of the polygon
                foreach (var edge in polygon)
                {
                    var newTriangle = new Triangle(edge.Item1, edge.Item2, point);
                    triangles.Add(newTriangle);
                }
            }

            // Remove triangles that contain vertices from the super triangle
            triangles.RemoveAll(t =>
                t.ContainsPoint(superTriangle.A) ||
                t.ContainsPoint(superTriangle.B) ||
                t.ContainsPoint(superTriangle.C));

            return triangles;
        }

        private static Triangle CreateSuperTriangle(List<Point> points)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            double dx = maxX - minX;
            double dy = maxY - minY;
            double deltaMax = Math.Max(dx, dy);
            double midX = (minX + maxX) / 2;
            double midY = (minY + maxY) / 2;

            var p1 = new Point(midX - 20 * deltaMax, midY - deltaMax, 0);
            var p2 = new Point(midX, midY + 20 * deltaMax, 0);
            var p3 = new Point(midX + 20 * deltaMax, midY - deltaMax, 0);

            return new Triangle(p1, p2, p3);
        }

        private static bool IsPointInCircumcircle(Triangle triangle, Point point)
        {
            // Calculate the circumcircle of the triangle and check if point is inside it
            Point a = triangle.A;
            Point b = triangle.B;
            Point c = triangle.C;

            double ab = a.X * a.X + a.Y * a.Y;
            double cd = b.X * b.X + b.Y * b.Y;
            double ef = c.X * c.X + c.Y * c.Y;

            double circumX = (ab * (c.Y - b.Y) + cd * (a.Y - c.Y) + ef * (b.Y - a.Y)) /
                            (a.X * (c.Y - b.Y) + b.X * (a.Y - c.Y) + c.X * (b.Y - a.Y)) / 2;

            double circumY = (ab * (c.X - b.X) + cd * (a.X - c.X) + ef * (b.X - a.X)) /
                            (a.Y * (c.X - b.X) + b.Y * (a.X - c.X) + c.Y * (b.X - a.X)) / 2;

            double circumRadius = Math.Sqrt((a.X - circumX) * (a.X - circumX) +
                                          (a.Y - circumY) * (a.Y - circumY));

            double pointDistance = Math.Sqrt((point.X - circumX) * (point.X - circumX) +
                                           (point.Y - circumY) * (point.Y - circumY));

            return pointDistance <= circumRadius;
        }

        // Alternative implementation using determinant method (more numerically stable)
        private static bool IsPointInCircumcircleDeterminant(Triangle triangle, Point point)
        {
            Point a = triangle.A;
            Point b = triangle.B;
            Point c = triangle.C;
            Point d = point;

            double[,] matrix = {
            {a.X - d.X, a.Y - d.Y, (a.X - d.X) * (a.X - d.X) + (a.Y - d.Y) * (a.Y - d.Y)},
            {b.X - d.X, b.Y - d.Y, (b.X - d.X) * (b.X - d.X) + (b.Y - d.Y) * (b.Y - d.Y)},
            {c.X - d.X, c.Y - d.Y, (c.X - d.X) * (c.X - d.X) + (c.Y - d.Y) * (c.Y - d.Y)}
        };

            // Calculate determinant of the matrix
            double det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[2, 1] * matrix[1, 2]) -
                        matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[2, 0] * matrix[1, 2]) +
                        matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[2, 0] * matrix[1, 1]);

            // For counter-clockwise triangles, point is inside circumcircle if determinant > 0
            return det > 0;
        }
    }
}
