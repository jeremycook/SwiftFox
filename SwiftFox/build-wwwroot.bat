cd wwwroot/

call npm install

robocopy ./node_modules/bootstrap-icons/ ./bootstrap-icons/ LICENSE.md
robocopy ./node_modules/bootstrap-icons/icons/ ./bootstrap-icons/icons/
