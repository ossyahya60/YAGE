#region File Description
//-----------------------------------------------------------------------------
// PrimitiveBatch.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
// NOTE: The entirety (mostly) of this code belongs to SimonDarksideJ
// here is the link to the code: https://github.com/SimonDarksideJ/XNAGameStudio/blob/archive/Samples/PrimitivesSample_4_0/Primitives/PrimitiveBatch.cs
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DataOrientedEngine.Engine
{

    // PrimitiveBatch is a class that handles efficient rendering automatically for its
    // users, in a similar way to SpriteBatch. PrimitiveBatch can render lines, points,
    // and triangles to the screen. In this sample, it is used to draw a spacewars
    // retro scene.
    public class PrimitiveBatch : IDisposable
    {
        #region Constants and Fields

        // this constant controls how large the vertices buffer is. Larger buffers will
        // require flushing less often, which can increase performance. However, having
        // buffer that is unnecessarily large will waste memory.
        const int DefaultBufferSize = 500;

        // a block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        VertexPositionColor[] vertices = new VertexPositionColor[DefaultBufferSize];

        // keeps track of how many vertices have been added. this value increases until
        // we run out of space in the buffer, at which time Flush is automatically
        // called.
        int positionInBuffer = 0;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        BasicEffect basicEffect;

        // the device that we will issue draw calls to.
        GraphicsDevice device;

        // this value is set by Begin, and is the type of primitives that we are
        // drawing.
        PrimitiveType primitiveType;

        // how many verts does each of these primitives take up? points are 1,
        // lines are 2, and triangles are 3.
        int numVertsPerPrimitive;

        // hasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        bool hasBegun = false;

        bool isDisposed = false;

        #endregion

        // the constructor creates a new PrimitiveBatch and sets up all of the internals
        // that PrimitiveBatch will need.
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            device = graphicsDevice;

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height, 0,
                0, 1);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                if (basicEffect != null)
                    basicEffect.Dispose();

                isDisposed = true;
            }
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType)
        {
            if (hasBegun)
            {
                throw new InvalidOperationException
                    ("End must be called before Begin can be called again.");
            }

            // these three types reuse vertices, so we can't flush properly without more
            // complex logic. Since that's a bit too complicated for this sample, we'll
            // simply disallow them.
            if (primitiveType == PrimitiveType.LineStrip ||
                primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException
                    ("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            this.primitiveType = primitiveType;

            // how many verts will each of these primitives require?
            this.numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);

            //tell our basic effect to begin.
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            hasBegun = true;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            bool newPrimitive = ((positionInBuffer % numVertsPerPrimitive) == 0);

            if (newPrimitive &&
                (positionInBuffer + numVertsPerPrimitive) >= vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.
            vertices[positionInBuffer].Position = new Vector3(vertex, 0);
            vertices[positionInBuffer].Color = color;

            positionInBuffer++;
        }

        // End is called once all the primitives have been drawn using AddVertex.
        // it will call Flush to actually submit the draw call to the graphics card, and
        // then tell the basic effect to end.
        public void End()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            Flush();

            hasBegun = false;
        }

        // Flush is called to issue the draw call to the graphics card. Once the draw
        // call is made, positionInBuffer is reset, so that AddVertex can start over
        // at the beginning. End will call this to draw the primitives that the user
        // requested, and AddVertex will call this if there is not enough room in the
        // buffer.
        private void Flush()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before Flush can be called.");
            }

            // no work to do
            if (positionInBuffer == 0)
            {
                return;
            }

            // how many primitives will we draw?
            int primitiveCount = positionInBuffer / numVertsPerPrimitive;

            // submit the draw call to the graphics card
            device.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0,
                primitiveCount);

            // now that we've drawn, it's ok to reset positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            positionInBuffer = 0;
        }

        #region Helper functions

        // NumVertsPerPrimitive is a boring helper function that tells how many vertices
        // it will take to draw each kind of primitive.
        static private int NumVertsPerPrimitive(PrimitiveType primitive)
        {
            int numVertsPerPrimitive;
            switch (primitive)
            {
                case PrimitiveType.LineList:
                    numVertsPerPrimitive = 2;
                    break;
                case PrimitiveType.TriangleList:
                    numVertsPerPrimitive = 3;
                    break;
                default:
                    throw new InvalidOperationException("primitive is not valid");
            }
            return numVertsPerPrimitive;
        }

        #endregion

        #region Shapes

        /// <summary>
        /// Draw a rectangle outline, PrimitiveType has to be LineList
        /// </summary>
        /// <param name="Destination rectangle"></param>
        /// <param name="color"></param>
        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            AddVertex(rectangle.Location.ToVector2(), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Top), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Top), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
            AddVertex(rectangle.Location.ToVector2(), color);
        }

        /// <summary>
        /// Draw rounded rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="roundedness ranges from 0 to 1, 0 is full rectangle, 1 is a complete circle"></param>
        /// <param name="color"></param>
        public void DrawRoundedRectangle(Rectangle rectangle, float roundedness, Color color)
        {
            int radius = (int)(MathHelper.Clamp(roundedness, 0, (float)MathHelper.Min(rectangle.Width, rectangle.Height) / MathHelper.Max(rectangle.Width, rectangle.Height)) * rectangle.Width * 0.5f);

            DrawArc(new Vector2(rectangle.Left + radius, rectangle.Top + radius), radius, (float)Math.PI, (float)Math.PI * 3 / 2, 32, color);
            DrawArc(new Vector2(rectangle.Right - radius, rectangle.Top + radius), radius, (float)Math.PI * 3 / 2, (float)Math.PI * 2, 32, color);
            DrawArc(new Vector2(rectangle.Right - radius, rectangle.Bottom - radius), radius, 0, (float)Math.PI / 2, 32, color);
            DrawArc(new Vector2(rectangle.Left + radius, rectangle.Bottom - radius), radius, (float)Math.PI / 2, (float)Math.PI, 32, color);

            AddVertex(new Vector2(rectangle.Left + radius, rectangle.Top), color);
            AddVertex(new Vector2(rectangle.Right - radius, rectangle.Top), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Top + radius), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Bottom - radius), color);
            AddVertex(new Vector2(rectangle.Right - radius, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Left + radius, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Left, rectangle.Bottom - radius), color);
            AddVertex(new Vector2(rectangle.Left, rectangle.Top + radius), color);
        }

        /// <summary>
        /// Draw a hollow circle, PrimitiveType has to be LineList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="numberOfLines aka quality"></param>
        /// <param name="color"></param>
        public void DrawCircle(Vector2 center, int radius, int numberOfLines, Color color)
        {
            float Theta = 0;
            float Increment = 2 * (float)Math.PI / numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
            }
        }

        /// <summary>
        /// Draw a shell of a circle that is hollow, PrimitiveType should be LineList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerRadius"></param>
        /// <param name="outerRadius"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawShellCircle(Vector2 center, int innerRadius, int outerRadius, int numberOfLines, Color color)
        {
            DrawCircle(center, innerRadius, numberOfLines, color);
            DrawCircle(center, outerRadius, numberOfLines, color);
        }

        /// <summary>
        /// Draw an arc, PrimitiveType should be LineList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawArc(Vector2 center, int radius, float startAngle, float endAngle, int numberOfLines, Color color)
        {
            float Theta = startAngle;
            float Increment = (endAngle - startAngle) / numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
            }
        }

        /// <summary>
        /// Draw a hollow shell arc, PrimitiveType should be LineList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerRadius"></param>
        /// <param name="outerRadius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawShellArc(Vector2 center, int innerRadius, int outerRadius, float startAngle, float endAngle, int numberOfLines, Color color)
        {
            DrawArc(center, innerRadius, startAngle, endAngle, numberOfLines, color);
            DrawArc(center, outerRadius, startAngle, endAngle, numberOfLines, color);
        }

        /// <summary>
        /// Draw a solid arc, PrimitiveType should be TriangleList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawSolidArc(Vector2 center, int radius, float startAngle, float endAngle, int numberOfLines, Color color)
        {
            float Theta = startAngle;
            float Increment = 2 * (float)Math.PI / numberOfLines;
            numberOfLines = (int)(numberOfLines * endAngle / (2 * (float)Math.PI));

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(Vector2.One * center, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
            }
        }

        /// <summary>
        /// Draw a shell of an arc, PrimitiveType should be TriangleList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerRadius"></param>
        /// <param name="outerRadius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawSolidHollowArc(Vector2 center, int innerRadius, int outerRadius, float startAngle, float endAngle, int numberOfLines, Color color)
        {
            float Theta = startAngle;
            float Increment = 2 * (float)Math.PI / numberOfLines;
            numberOfLines = (int)(numberOfLines * endAngle / (2 * (float)Math.PI));

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(center + new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * innerRadius, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius + center, color);
                float thetaPrev = Theta;
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius + center, color);

                AddVertex(center + new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius, color);
                AddVertex(new Vector2((float)Math.Cos(thetaPrev), (float)Math.Sin(thetaPrev)) * innerRadius + center, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * innerRadius + center, color);
            }
        }

        /// <summary>
        /// Draw a solid circle, PrimitiveType has to be TriangleList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="numberOfLines aka quality"></param>
        /// <param name="color"></param>
        public void DrawSolidCircle(Vector2 center, int radius, int numberOfLines, Color color)
        {
            float Theta = 0;
            float Increment = 2 * (float)Math.PI / numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(Vector2.One * center, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * radius + center, color);
            }
        }

        /// <summary>
        /// Draw a shell of a circle that is solid, PrimitiveType should be TriangleList
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerRadius"></param>
        /// <param name="outerRadius"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="color"></param>
        public void DrawSolidShellCircle(Vector2 center, int innerRadius, int outerRadius, int numberOfLines, Color color)
        {
            float Theta = 0;
            float Increment = 2 * (float)Math.PI / numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                AddVertex(center + new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * innerRadius, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius + center, color);
                float thetaPrev = Theta;
                Theta += Increment;
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius + center, color);

                AddVertex(center + new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * outerRadius, color);
                AddVertex(new Vector2((float)Math.Cos(thetaPrev), (float)Math.Sin(thetaPrev)) * innerRadius + center, color);
                AddVertex(new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * innerRadius + center, color);
            }
        }

        /// <summary>
        /// Draw a solid rectangle, PrimitiveType has to be TriangleList
        /// </summary>
        /// <param name="Destination rectangle"></param>
        /// <param name="color"></param>
        public void DrawSolidRectangle(Rectangle rectangle, Color color)
        {
            AddVertex(rectangle.Location.ToVector2(), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Top), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
            AddVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
            AddVertex(rectangle.Location.ToVector2(), color);
        }

        #endregion
    }
}