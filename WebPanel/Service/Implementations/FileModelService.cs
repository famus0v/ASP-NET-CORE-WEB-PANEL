using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using TryWebSite.Domain.Enum;
using WebPanel.DAL.Interfaces;
using WebPanel.DAL.Repositories;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Enum;
using WebPanel.Domain.Response;
using WebPanel.Misc;
using WebPanel.Service.Interfaces;

namespace WebPanel.Service.Implementations
{
    public class FileModelService : IFileModelService
    {
        private readonly IBaseRepository<FileModel> _filemodelRepository;
        private readonly IBaseRepository<TableElement> _elementsRepository;

        public FileModelService(IBaseRepository<FileModel> filemodelRepository, IBaseRepository<TableElement> elementsRepository)
        {
            _filemodelRepository = filemodelRepository;
            _elementsRepository = elementsRepository;
        }

        public async Task<IBaseResponse<FileModel>> AddFile(int id,FileType fileType, IFormFile fileModel)
        {
            var baseResponse = new BaseResponse<FileModel>();
            try
            {
                var element = await _elementsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (element == null)
                {
                    baseResponse.Description = "Element not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                
                    var stream = fileModel.OpenReadStream();

                    bool fileSaved = await PanelFilesManager.SaveFile(stream, fileModel.FileName, element.TableId.ToString(), element.FullName);

                    stream.Close();

                if (!fileSaved)
                {

                    Console.WriteLine("Ошибка в PanelFilesManager при копировании файла");

                    baseResponse.Description = "Error stream";
                    baseResponse.StatusCode = StatusCode.InternalServerError; return baseResponse;
                }



                var newFile = new FileModel()
                {
                    FileName = fileModel.FileName,
                    FilePath = $"{PanelFilesManager.MAIN_PATH}/{element.TableId}/{element.FullName}/{fileModel.FileName}",
                    FileType = fileType
                    
                };

                await _filemodelRepository.Add(newFile);

                if(fileType == FileType.Connections)
                if (element.ConnectionsId == null)
                    element.ConnectionsId = JsonListConverter.CreateNewList(newFile.Id);
                else
                    element.ConnectionsId = JsonListConverter.AddToStringList(element.ConnectionsId, newFile.Id);

                if (fileType == FileType.Picture)
                    if (element.PictureId == null)
                        element.PictureId = JsonListConverter.CreateNewList(newFile.Id);
                    else
                        element.PictureId = JsonListConverter.AddToStringList(element.PictureId, newFile.Id);

                if (fileType == FileType.IllegalContent)
                    if (element.IllegalContentId == null)
                        element.IllegalContentId = JsonListConverter.CreateNewList(newFile.Id);
                    else
                        element.IllegalContentId = JsonListConverter.AddToStringList(element.IllegalContentId, newFile.Id);

                if (fileType == FileType.Landing)
                    if (element.LandingId == null)
                        element.LandingId = JsonListConverter.CreateNewList(newFile.Id);
                    else
                        element.LandingId = JsonListConverter.AddToStringList(element.LandingId, newFile.Id);

                await _elementsRepository.Update(element);

                baseResponse.Data = newFile;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<FileModel>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<FileModel>> DeleteFile(int elementId, int id)
        {
            var baseResponse = new BaseResponse<FileModel>();
            try
            {
                var file = await _filemodelRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (file == null)
                {
                    baseResponse.Description = "Element not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                var element = await _elementsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == elementId);


                if (file.FileType== FileType.Connections)
                    if (element.ConnectionsId != null)
                        element.ConnectionsId = JsonListConverter.DeleteIntoList(element.ConnectionsId, file.Id);

                if (file.FileType == FileType.Picture)
                    if (element.PictureId != null)
                        element.PictureId = JsonListConverter.DeleteIntoList(element.PictureId, file.Id);

                if (file.FileType == FileType.IllegalContent)
                    if (element.IllegalContentId != null)
                        element.IllegalContentId = JsonListConverter.DeleteIntoList(element.IllegalContentId, file.Id);

                if (file.FileType == FileType.Landing)
                    if (element.LandingId != null)
                        element.LandingId = JsonListConverter.DeleteIntoList(element.LandingId, file.Id);


                PanelFilesManager.DeleteFile(file.FilePath);
                await _filemodelRepository.Delete(file);
                await _elementsRepository.Update(element);

                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<FileModel>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<FileModel>> GetFile(int id)
        {
            var baseResponse = new BaseResponse<FileModel>();
            try
            {
                var resource = await _filemodelRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (resource == null)
                {
                    baseResponse.Description = "File not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<FileModel>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public Task<IBaseResponse<IEnumerable<FileModel>>> GetFiles(string json)
        {
            throw new NotImplementedException();
        }
    }
}
