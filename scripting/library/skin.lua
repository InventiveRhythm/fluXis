---@meta

---@class skin
skin = {}

---gets the aspect ratio of a skin sprite. returns 1 if not available
---@param str SkinSprite
---@return number
---@nodiscard
function skin:sprratio(str) end

---@param mode number
---@return number
---@nodiscard
function skin:colwidth(mode) end

---@param mode number
---@return number
---@nodiscard
function skin:hitpos(mode) end

---@param mode number
---@return number
---@nodiscard
function skin:recoffset(mode) end

---@param mode number
---@return boolean
---@nodiscard
function skin:recfirst(mode) end