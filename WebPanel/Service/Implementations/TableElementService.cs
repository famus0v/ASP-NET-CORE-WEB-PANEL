using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TryWebSite.Domain.Enum;
using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;
using WebPanel.Domain.ViewModels;
using WebPanel.Misc;
using WebPanel.Service.Interfaces;

namespace WebPanel.Service.Implementations
{
    public class TableElementService : ITableElementService
    {
        private readonly IBaseRepository<TableElement> _elementRepository;
        private readonly IBaseRepository<Account> _accountRepository;
        private readonly IBaseRepository<FileModel> _filemodelRepository;

        public TableElementService(IBaseRepository<TableElement> elementRepository, IBaseRepository<Account> accountRepository, IBaseRepository<FileModel> filemodelRepository)
        {
            _elementRepository = elementRepository;
            _accountRepository = accountRepository;
            _filemodelRepository = filemodelRepository;
        }

        public async Task<IBaseResponse<TableElement>> AddElement(TableElement elementViewModel,int tableId, string userIdentityName)
        {
            try
            {
                var allElements = _elementRepository.GetAll();
                var probableName = await allElements
                    .FirstOrDefaultAsync(x => x.FullName.ToLower() == elementViewModel.FullName.ToLower());

                if (probableName != null)
                {
                    return new BaseResponse<TableElement>()
                    {
                        Description = "Нельзя использовать существующее имя",
                        StatusCode = StatusCode.ProbableName
                    };
                }

                PanelFilesManager.CheckElementPath(tableId.ToString(), elementViewModel.FullName);
                elementViewModel.ElementPath = $"{PanelFilesManager.MAIN_PATH}/{tableId}/{elementViewModel.FullName}";

                var ownerUser = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userIdentityName);
                string ownerName = ownerUser?.Name ?? "unknown";
                string ownerFullName = ownerUser?.FullName ?? "unknown";

                var resource = new TableElement()
                {
                    TableId = tableId,

                    FullName = elementViewModel.FullName,
                    City = elementViewModel.City,
                    Link = elementViewModel.Link,
                    Note = elementViewModel.Note,
                    Interests = elementViewModel.Interests,
                    //PhoneNumber = elementViewModel.PhoneNumber,
                    NegativeEmotStates = elementViewModel.NegativeEmotStates,
                    Gender = elementViewModel.Gender,
                    Color = elementViewModel.Color,

                    InformationTransferred = elementViewModel.InformationTransferred,

                    DateOfBirth = elementViewModel.DateOfBirth,
                    DateOfDetection = elementViewModel.DateOfDetection,
                    DateOfChanges = DateTime.Now,

                    ConnectionsId = elementViewModel.ConnectionsId,

                    ElementPath = elementViewModel.ElementPath,

                    
                    OwnerName = ownerName,
                    OwnerFullName = ownerFullName,
                };

                await _elementRepository.Add(resource);

                return new BaseResponse<TableElement>()
                {
                    StatusCode = StatusCode.OK,
                    Data = resource
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TableElement>()
                {
                    Description = $"[Create] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteElement(int id)
        {
            var baseResponse = new BaseResponse<bool>();
            try
            {
                var element = await _elementRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

                if (element == null)
                {
                    baseResponse.Description = "Resource not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                if(element.ElementPath != null)
                PanelFilesManager.DeleteFilePath(element.ElementPath);

                List<int> fileIds = new();

                if(element.ConnectionsId != null)
                    fileIds.AddRange(JsonListConverter.GetListIntoString(element.ConnectionsId));
                if (element.PictureId != null)
                    fileIds.AddRange(JsonListConverter.GetListIntoString(element.PictureId));
                if (element.IllegalContentId != null)
                    fileIds.AddRange(JsonListConverter.GetListIntoString(element.IllegalContentId));

                List<FileModel> filesList = _filemodelRepository.GetAll().Where(x => fileIds.Contains(x.Id)).ToList();

                await _filemodelRepository.DeleteRange(filesList);

                await _elementRepository.Delete(element);
                baseResponse.Data = true;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[DeleteResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<TableElement>> EditElement(TableElement elementViewModel)
        {
            try
            {
                var allElements = _elementRepository.GetAll();

                var resource = await allElements.FirstOrDefaultAsync(x => x.Id == elementViewModel.Id);
                if (resource == null)
                {
                    return new BaseResponse<TableElement>()
                    {
                        Description = "Element not found",
                        StatusCode = StatusCode.ElementNotFound
                    };
                }

                if (!resource.FullName.Contains(elementViewModel.FullName, StringComparison.OrdinalIgnoreCase)) {
                    var probableName = await allElements
                    .FirstOrDefaultAsync(x => x.FullName.ToLower() == elementViewModel.FullName.ToLower());
                    if (probableName != null)
                    {
                        return new BaseResponse<TableElement>()
                        {
                            Description = "Нельзя использовать существующее имя",
                            StatusCode = StatusCode.ProbableName
                        };
                    }
                }

                if(resource.FullName != elementViewModel.FullName)
                {
                    var oldPath = resource.ElementPath;
                    var newPath = $"{PanelFilesManager.MAIN_PATH}/{resource.TableId}/{elementViewModel.FullName}";
                    resource.ElementPath = newPath;
                    if (Directory.Exists(oldPath))
                        Directory.Move(oldPath, newPath);


                    List<int> fileIds = new();

                    if (resource.ConnectionsId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(resource.ConnectionsId));
                    if (resource.PictureId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(resource.PictureId));
                    if (resource.IllegalContentId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(resource.IllegalContentId));

                    List<FileModel> filesList = _filemodelRepository.GetAll().Where(x => fileIds.Contains(x.Id)).ToList();

                    foreach (var fileModel in filesList) {
                        fileModel.FilePath = newPath + "/" + fileModel.FileName;
                        await _filemodelRepository.Update(fileModel);
                            }
                }

                resource.FullName = elementViewModel.FullName;
                resource.City = elementViewModel.City;
                resource.Link = elementViewModel.Link;
                resource.Note = elementViewModel.Note;
                //resource.PhoneNumber = elementViewModel.PhoneNumber;
                resource.NegativeEmotStates = elementViewModel.NegativeEmotStates;
                resource.Gender = elementViewModel.Gender;
                resource.Color = elementViewModel.Color;
                resource.Interests = elementViewModel.Interests;

                resource.InformationTransferred = elementViewModel.InformationTransferred;

                resource.DateOfBirth = elementViewModel.DateOfBirth;
                resource.DateOfDetection = elementViewModel.DateOfDetection;
                resource.DateOfChanges = DateTime.Now;

                await _elementRepository.Update(resource);


                return new BaseResponse<TableElement>()
                {
                    Data = resource,
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TableElement>()
                {
                    Description = $"[Edit] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TableElement>>> GetAllElements()
        {
            try
            {
                var resources = await _elementRepository.GetAll().ToListAsync();
                if (!resources.Any())
                {
                    return new BaseResponse<IEnumerable<TableElement>>()
                    {
                        Description = "Найдено 0 элементов",
                        StatusCode = StatusCode.OK
                    };
                }

                return new BaseResponse<IEnumerable<TableElement>>()
                {
                    Data = resources,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<TableElement>>()
                {
                    Description = $"[GetResources] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<TableElement>> GetElementById(int id)
        {
            var baseResponse = new BaseResponse<TableElement>();
            try
            {
                var resource = await _elementRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (resource == null)
                {
                    baseResponse.Description = "Resource not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<TableElement>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<TableElement>> GetElementByName(string name)
        {
            var baseResponse = new BaseResponse<TableElement>();
            try
            {
                var resource = await _elementRepository.GetAll().FirstOrDefaultAsync(x => x.FullName == name);
                if (resource == null)
                {
                    baseResponse.Description = "Resource not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<TableElement>()
                {
                    Description = $"[GetResourceByName] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TableElement>>> GetElements(FindViewModel viewModel)
        {
            try
            {
                var allElements = await _elementRepository.GetAll().ToListAsync();
                List<TableElement> elements = new();

                //foreach(var element in allElements)
                //{
                //    if (element.TableId != viewModel.TableId) continue;

                //    if (viewModel.Id != null)
                //        if (element.Id != viewModel.Id) continue;

                //    if (viewModel.Name != null && element.FullName != null)
                //        if (!element.FullName.Contains(viewModel.Name, StringComparison.OrdinalIgnoreCase)) continue;

                //    if (viewModel.City != null && element.City != null)
                //        if (!element.City.Contains(viewModel.City, StringComparison.OrdinalIgnoreCase)) continue;

                //    if (viewModel.PhoneNumber != null && element.PhoneNumber != null)
                //        if (!element.PhoneNumber.Contains(viewModel.PhoneNumber, StringComparison.OrdinalIgnoreCase)) continue;

                //    if (viewModel.NegativeEmotStates != null && element.NegativeEmotStates != null)
                //        if (!element.NegativeEmotStates.Contains(viewModel.NegativeEmotStates, StringComparison.OrdinalIgnoreCase)) continue;

                //    if(viewModel.Gender!= null)
                //    if (viewModel.Gender != Domain.Enum.Genders.NoGender)
                //        if (element.Gender != viewModel.Gender) continue;

                //    if (viewModel.Color != null)
                //    if (viewModel.Color != Domain.Enum.Colors.NoColor)
                //        if (element.Color != viewModel.Color) continue;

                //    if (viewModel.DateOfBirth != null && element.DateOfBirth != null)
                //        if (element.DateOfBirth != viewModel.DateOfBirth) continue;

                //    if (viewModel.DateOfDetection != null && element.DateOfDetection != null)
                //        if (element.DateOfDetection != viewModel.DateOfDetection) continue;

                //    if (viewModel.DateOfChanges != null && element.DateOfChanges != null)
                //        if (element.DateOfChanges != viewModel.DateOfChanges) continue;

                //    if (viewModel.Owner != null && element.Owner != null)
                //        if (!element.Owner.Contains(viewModel.Owner, StringComparison.OrdinalIgnoreCase)) continue;

                //    elements.Add(element);
                //}

                var filteredElements = allElements.Where(element =>
                element.TableId == viewModel.TableId &&
                (viewModel.Id == null || element.Id == viewModel.Id) &&
                (viewModel.Name == null || (element.FullName != null && element.FullName.Contains(viewModel.Name, StringComparison.OrdinalIgnoreCase))) &&
                (viewModel.City == null || (element.City != null && element.City.Contains(viewModel.City, StringComparison.OrdinalIgnoreCase))) &&
                //(viewModel.PhoneNumber == null || (element.PhoneNumber != null && element.PhoneNumber.Contains(viewModel.PhoneNumber, StringComparison.OrdinalIgnoreCase))) &&
                (viewModel.NegativeEmotStates == null || (element.NegativeEmotStates != null && element.NegativeEmotStates.Contains(viewModel.NegativeEmotStates, StringComparison.OrdinalIgnoreCase))) &&
                (viewModel.Interests == null || (element.Interests != null && element.Interests.Contains(viewModel.Interests, StringComparison.OrdinalIgnoreCase))) &&
                (viewModel.Gender == null || viewModel.Gender == Domain.Enum.Genders.NoGender || element.Gender == viewModel.Gender) &&
                (viewModel.Color == null || viewModel.Color == Domain.Enum.Colors.NoColor || element.Color == viewModel.Color) &&
                (viewModel.DateOfBirth == null || element.DateOfBirth?.Date == viewModel.DateOfBirth?.Date) &&
                (viewModel.DateOfDetection == null || element.DateOfDetection?.Date == viewModel.DateOfDetection?.Date) &&
                (viewModel.DateOfChanges == null || element.DateOfChanges?.Date == viewModel.DateOfChanges?.Date) &&
                (viewModel.Owner == null || (element.OwnerFullName != null && element.OwnerFullName.Contains(viewModel.Owner, StringComparison.OrdinalIgnoreCase)))
                ).ToList();

                elements.AddRange(filteredElements);

                if (!elements.Any() || elements.IsNullOrEmpty())
                {
                    return new BaseResponse<IEnumerable<TableElement>>()
                    {
                        Description = "Найдено 0 элементов",
                        StatusCode = StatusCode.ElementNotFound
                    };
                }

                return new BaseResponse<IEnumerable<TableElement>>()
                {
                    Data = elements,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<TableElement>>()
                {
                    Description = $"[GetElements] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TableElement>>> GetElementsByTableId(int tableId)
        {
            var baseResponse = new BaseResponse<IEnumerable<TableElement>>();
            try
            {
                var resource = await _elementRepository.GetAll().Where(x => x.TableId == tableId).ToListAsync();
                if (resource == null)
                {
                    baseResponse.Description = "Resources not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<TableElement>>()
                {
                    Description = $"[GetElementsByTableId] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
