using System;
using OpenTK.Graphics.OpenGL4;
using LGL.Loaders;

namespace LGL.Gfx
{
    public class Pipeline : IDisposable
    {
        private int id;
        private Shader shader;

        public Pipeline()
        {
            GL.CreateProgramPipelines(1, out id);
        }

        public void Dispose()
        {
            GL.DeleteProgramPipeline(id);
        }

        public void Set()
        {
            GL.BindProgramPipeline(id);
        }

        public void Use(Shader program, ProgramStageMask stage)
        {
            shader = program;
            GL.UseProgramStages(id, stage, program.ShaderID);
        }
    }
}
