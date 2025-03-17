@echo off
echo Creating .gitkeep files...
for /f "tokens=* delims=" %%n in ('dir "%cd%" /b/ad/s') do (
    dir/a/b "%%n\"|findstr . >nul&&(
        rem 不执行任何操作
    )||(
        copy nul "%%n\.gitkeep" >nul
        echo Created .gitkeep in "%%n"
    )
)
echo Done.
pause