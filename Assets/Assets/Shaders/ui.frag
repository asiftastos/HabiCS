#version 330 core

in vec2 oTex;

uniform vec4 color;
uniform bool text;

out vec4 FragColor;

uniform sampler2D mytexture;

const float smoothing = 1.0/16.0;

void drawText()
{
    float distance = texture(mytexture, oTex).r;
    float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, distance);
    FragColor = vec4(color.xyz, alpha);
}

void main()
{
    if(text)
    {
        drawText();
    }else {
        FragColor = color;
    }
}
