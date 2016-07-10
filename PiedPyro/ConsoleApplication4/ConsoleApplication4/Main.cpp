#include<conio.h>
#include<graphics.h>

void main()
{
	clrscr();
	int driver = DETECT,mode;
	initgraph(&driver, &mode, "C:\\tc\\bgi");
	putpix4el(300, 200, WHITE);
	getch();
	closegraph();
}